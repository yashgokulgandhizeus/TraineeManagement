using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TraineeManagement.Worker.Messaging;

namespace TraineeManagement.Worker;

public class SubmissionProcessorWorker : BackgroundService
{
    private readonly ILogger<SubmissionProcessorWorker> _logger;
    private readonly RabbitMqSettings _rabbitSettings;
    private IConnection? _connection;
    private IChannel? _channel;

    public SubmissionProcessorWorker(ILogger<SubmissionProcessorWorker> logger, IOptions<RabbitMqSettings> rabbitSettings)
    {
        _logger = logger;
        _rabbitSettings = rabbitSettings.Value;
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

        // 🟢 v7 Asynchronous connection and channel allocation pattern
        _connection = await factory.CreateConnectionAsync(stoppingToken);
        _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await _channel.QueueDeclareAsync(
            queue: "submission-processing",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: stoppingToken);

        // Quality of Service: Consume exactly one message at a time initially
        await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: stoppingToken);

        // 🟢 v7 Async Consumer engine syntax setup
        var consumer = new AsyncEventingBasicConsumer(_channel);
        
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageString = Encoding.UTF8.GetString(body);
            
            try
            {
                var taskRequest = JsonSerializer.Deserialize<SubmissionProcessingRequested>(messageString);

                if (taskRequest != null)
                {
                    _logger.LogInformation("Processing Message Context. MessageId: {MsgId}, FileId: {FileId}", 
                        taskRequest.MessageId, taskRequest.FileId);

                    // --- PLACE YOUR COMPLEX BACKGROUND FILE ANALYSIS LOGIC HERE ---
                    await Task.Delay(2000, stoppingToken); // Simulating processing work execution
                    // -------------------------------------------------------------

                    _logger.LogInformation("Work task completed successfully. Sending Positive Acknowledgment (Ack).");
                    
                    // 🟢 v7 manual async acknowledgement sequence
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during workflow compilation inside the background agent loop.");

                // Negatively Acknowledge (Nack) and requeue the message so it doesn't get lost
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "submission-processing",
            autoAck: false, // Must be false for manual acknowledgments to function safely
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
