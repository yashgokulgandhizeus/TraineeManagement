using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client; // Core namespace containing ConnectionFactory, IConnection, IChannel
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Messaging;
using TraineeManagement.Api.Models;


namespace TraineeManagement.Api.Services;

public class SubmissionFileService : ISubmissionFileService
{
    private readonly AppDbContext _context;
    private readonly IFileStorageService _storage;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SubmissionFileService> _logger;
    private readonly RabbitMqSettings _rabbitSettings;

    public SubmissionFileService(
        AppDbContext context,
        IFileStorageService storage,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor,
        ILogger<SubmissionFileService> logger,
        IOptions<RabbitMqSettings> rabbitSettings)
    {
        _context = context;
        _storage = storage;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _rabbitSettings = rabbitSettings.Value;
    }

    public async Task<UploadFileResponse?> UploadAsync(int submissionId, IFormFile file)
    {
        var submission = await _context.Submissions.FirstOrDefaultAsync(s => s.Id == submissionId);
        if (submission == null) return null;
        if (file.Length == 0) throw new ArgumentException("Uploaded file contains no binary length data.");

        string[] allowedExtensions = _configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>()!;
        string extension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(extension)) throw new BadHttpRequestException("Invalid file extension context.");

        long maxSize = _configuration.GetValue<long>("FileStorage:MaxSizeMB") * 1024 * 1024;
        if (file.Length > maxSize) throw new BadHttpRequestException("File context size parameters exceed systemic maximum allocation thresholds.");

        string storedFileName = await _storage.SaveFileAsync(file);
        string checksum = await GenerateChecksum(file);

        var user = _httpContextAccessor.HttpContext!.User;
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var username = user.FindFirst(ClaimTypes.Name)!.Value;

        var submissionFile = new SubmissionFile
        {
            SubmissionId = submissionId,
            OriginalFileName = file.FileName,
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            Checksum = checksum,
            UploadedByUser = username,
            UploadedByUserId = userId,
            UploadedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow
        };

        _context.SubmissionFiles.Add(submissionFile);
        await _context.SaveChangesAsync();

        var correlationId = Guid.NewGuid();

        var processingJob = new ProcessingJob
        {
            CorrelationId = correlationId,
            FileId = submissionFile.Id,
            Status = JobStatus.Queued,
            Attempts = 0
        };
        _context.ProcessingJobs.Add(processingJob);
        await _context.SaveChangesAsync();

        var messageContract = new SubmissionProcessingRequested
        {
            CorrelationId = correlationId,
            SubmissionId = submissionId,
            FileId = submissionFile.Id
        };

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _rabbitSettings.Host,
                Port = _rabbitSettings.Port,
                VirtualHost = _rabbitSettings.VirtualHost,
                UserName = _rabbitSettings.Username,
                Password = _rabbitSettings.Password
            };


            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange: "dlx.submission", type: ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue: "submission-processing-poison", durable: true, exclusive: false, autoDelete: false);
            await channel.QueueBindAsync(queue: "submission-processing-poison", exchange: "dlx.submission", routingKey: "poison");

            var queueArguments = new Dictionary<string, object?>
              {
                  { "x-dead-letter-exchange", "dlx.submission" },
                  { "x-dead-letter-routing-key", "poison" }
              };

            await channel.QueueDeclareAsync(
                queue: "submission-processing",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArguments);

            var jsonPayload = JsonSerializer.Serialize(messageContract);
            var body = Encoding.UTF8.GetBytes(jsonPayload);

            // 🟢 v7 Properties allocation style - Persistent is natively a direct property boolean flag
            var properties = new BasicProperties
            {
                Persistent = true
            };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "submission-processing",
                mandatory: true,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Message published to broker layout stack. MessageId: {MessageId}, CorrelationId: {CorrelationId}",
                messageContract.MessageId, correlationId);
        }
        catch (Exception ex)
{
    // 🟢 CHANGED: This will print the EXACT root cause in your console logs
    _logger.LogError(ex, "CRITICAL: RabbitMQ connection failed. Real underlying error: {Message}", ex.Message);
    
    _context.SubmissionFiles.Remove(submissionFile);
    await _context.SaveChangesAsync();

    // Re-throw the real error so it shows up in your browser/Postman response
    throw; 
}


        return new UploadFileResponse
        {
            Id = submissionFile.Id,
            OriginalFileName = submissionFile.OriginalFileName,
            StoredFileName = submissionFile.StoredFileName,
            FileSize = submissionFile.FileSize,
            ContentType = submissionFile.ContentType,
            Checksum = submissionFile.Checksum,
            CorrelationId = correlationId
        };
    }

    public async Task<(Stream stream, string contentType, string fileName)?> DownloadAsync(int fileId)
    {
        var file = await _context.SubmissionFiles.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file == null) return null;

        bool exists = await _storage.ExistsAsync(file.StoredFileName);
        if (!exists) return null;

        var stream = await _storage.OpenReadAsync(file.StoredFileName);
        return (stream, file.ContentType, file.OriginalFileName);
    }

    public async Task<bool> DeleteAsync(int fileId)
    {
        var file = await _context.SubmissionFiles.FirstOrDefaultAsync(x => x.Id == fileId);
        if (file == null) return false;

        await _storage.DeleteAsync(file.StoredFileName);
        _context.SubmissionFiles.Remove(file);
        await _context.SaveChangesAsync();

        return true;
    }

    private static async Task<string> GenerateChecksum(IFormFile file)
    {
        using var sha256 = SHA256.Create();
        using var stream = file.OpenReadStream();
        byte[] hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToHexString(hash);
    }
}
