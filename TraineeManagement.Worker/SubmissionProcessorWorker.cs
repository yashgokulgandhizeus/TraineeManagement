using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Contracts;
using TraineeManagement.Worker.Messaging;

namespace TraineeManagement.Worker;

public class SubmissionProcessorWorker : BackgroundService
{
    private readonly ILogger<SubmissionProcessorWorker> _logger;
    private readonly RabbitMqSettings _rabbitSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory; 
    private IConnection? _connection;
    private IChannel? _channel;

    public SubmissionProcessorWorker(
        ILogger<SubmissionProcessorWorker> logger, 
        IOptions<RabbitMqSettings> rabbitSettings,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _rabbitSettings = rabbitSettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _rabbitSettings.Host,
            Port = _rabbitSettings.Port,
            VirtualHost = _rabbitSettings.VirtualHost,
            UserName = _rabbitSettings.Username,
            Password = _rabbitSettings.Password
        };

        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        var queueArguments = new Dictionary<string, object?>
        {
            { "x-dead-letter-exchange", "dlx.submission" },
            { "x-dead-letter-routing-key", "poison" }
        };

        await _channel.QueueDeclareAsync(
            queue: "submission-processing",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: queueArguments, 
            cancellationToken: stoppingToken);

        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            
            _logger.LogInformation("[Worker Traffic Intercepted] Raw Payload: {Data}", messageString);
            
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            SubmissionProcessingRequested? taskRequest = null;

            try
            {
                var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                taskRequest = JsonSerializer.Deserialize<SubmissionProcessingRequested>(messageString, jsonOptions);
                
                if (taskRequest == null) 
                    throw new FormatException("The JSON message could not be parsed into a clean contract object.");

                var trackedJob = await dbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.CorrelationId == taskRequest.CorrelationId, cancellationToken: stoppingToken);
                
                if (trackedJob == null)
                {
                    _logger.LogWarning("Job not explicitly initialized by API layer. Creating fallback track.");
                    trackedJob = new ProcessingJob { CorrelationId = taskRequest.CorrelationId, FileId = taskRequest.FileId, Status = JobStatus.Queued };
                    dbContext.ProcessingJobs.Add(trackedJob);
                    await dbContext.SaveChangesAsync(stoppingToken);
                }

                if (trackedJob.Status == JobStatus.Completed)
                {
                    _logger.LogWarning("Idempotency Triggered: Message CorrelationId {Id} was already completed.", taskRequest.CorrelationId);
                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    return;
                }

                trackedJob.Status = JobStatus.Processing;
                trackedJob.Attempts++;
                trackedJob.StartedAt = DateTime.UtcNow;
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Processing Job started. CorrelationId: {CorrId}, Attempt No: {Att}", trackedJob.CorrelationId, trackedJob.Attempts);

                var fileRecord = await dbContext.SubmissionFiles.FirstOrDefaultAsync(f => f.Id == taskRequest.FileId, cancellationToken: stoppingToken);
                if (fileRecord == null)
                {
                    throw new InvalidOperationException($"Permanent Error: Specified file entry metadata index {taskRequest.FileId} does not exist.");
                }

                // Simulate processing delay
                await Task.Delay(3000, stoppingToken); 

                trackedJob.Status = JobStatus.Completed;
                trackedJob.CompletedAt = DateTime.UtcNow;
                trackedJob.ErrorSummary = null; 
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("Asynchronous workload completed successfully for CorrelationId: {Id}.", taskRequest.CorrelationId);
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                // 🟢 FIXED: This will now catch and print out ANY exception that causes the freeze
                _logger.LogError(ex, "🔴 CRITICAL BACKGROUND WORKER CRASH: {ErrorMessage}", ex.Message);

                if (taskRequest != null)
                {
                    var trackedJob = await dbContext.ProcessingJobs.FirstOrDefaultAsync(j => j.CorrelationId == taskRequest.CorrelationId, cancellationToken: stoppingToken);
                    if (trackedJob != null)
                    {
                        bool isPermanentError = ex.Message.StartsWith("Permanent Error");
                        trackedJob.ErrorSummary = $"[Attempt {trackedJob.Attempts}] - {ex.Message}";

                        if (isPermanentError || trackedJob.Attempts >= 3)
                        {
                            trackedJob.Status = JobStatus.Failed;
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogCritical("Job processing failed permanently for CorrelationId: {Id}. Dead-lettering message.", taskRequest.CorrelationId);
                            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                        }
                        else
                        {
                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogWarning("Transient failure registered. Requeuing message back to broker line.");
                            await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                        }
                    }
                }
                else
                {
                    // If JSON deserialization completely failed, discard it from main queue into poison queue
                    _logger.LogCritical("Discarding malformed message string from queue to prevent loop blockages.");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "submission-processing",
            autoAck: false, 
            consumer: consumer,
            cancellationToken: stoppingToken);

        _logger.LogInformation("RabbitMQ background consumer started successfully. Listening for tasks...");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null) await _channel.CloseAsync(cancellationToken);
        if (_connection != null) await _connection.CloseAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}
