using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Exceptions;

namespace TraineeManagement.Api.Services;

public class SubmissionService : ISubmissionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SubmissionService> _logger;

    private readonly IConfiguration _config;
    private readonly ICacheService _cache;

    public SubmissionService(AppDbContext context, ILogger<SubmissionService> logger, IConfiguration config, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _cache = cache;
    }

    public async Task<SubmissionResponse> Create(SubmissionRequest request)
    {
        TaskAssignment taskAssignment = await _context.TaskAssignments.FirstOrDefaultAsync(e => e.Id == request.TaskAssignmentId);

        if (taskAssignment == null)
        {
            _logger.LogCritical("Failed to create submission. TaskAssignment not found with Id: " + request.TaskAssignmentId);

            throw new NotFoundException($"Task assignment context with Id {request.TaskAssignmentId} was not found.");
        }

        Submission submission = new Submission
        {
            TaskAssignment = taskAssignment,
            Status = request.Status,
            Notes = request.Notes,
            SubmissionUrl = request.SubmissionUrl
        };

        _context.Submissions.Add(submission);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Submission created successfully with Status: " + submission.Status + ", assigned Id: " + submission.Id + " for TaskAssignmentId: " + submission.TaskAssignmentId);

        var trackedKeys = await _cache.GetTAsync<List<string>>("submission:list:registry");
        if (trackedKeys != null && trackedKeys.Count > 0)
        {
            foreach (var key in trackedKeys)
            {

                if (key.Contains(":list:"))
                {
                    await _cache.RemoveAsync(key);
                }
            }
            _logger.LogInformation("Physically cleared {Count} task assignment keys from cache.", trackedKeys.Count);
        }



        return new SubmissionResponse
        {
            Id = submission.Id,
            Status = submission.Status,
            Notes = submission.Notes,
            SubmissionUrl = submission.SubmissionUrl,
            SubmittedDate = submission.SubmittedDate,
            TaskAssignmentId = submission.TaskAssignmentId
        };
    }

    public async Task<List<SubmissionResponse>> GetAll()
    {
        string cacheKey = CacheKeys.SubmissionList();

        var cachedData = await _cache.GetTAsync<List<SubmissionResponse>>(cacheKey);

        if (cachedData != null)
        {
            _logger.LogInformation("Cache HIT - Returning Task Submission.");
            return cachedData;
        }

        _logger.LogInformation("Cache MISS - Querying database.");

        _logger.LogInformation("Fetching all Submission from the database.");

        var response = await _context.Submissions.Select(e => new SubmissionResponse
        {
            Id = e.Id,
            Status = e.Status,
            Notes = e.Notes,
            SubmissionUrl = e.SubmissionUrl,
            SubmittedDate = e.SubmittedDate,
            TaskAssignmentId = e.TaskAssignmentId
        }).ToListAsync();

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return response;
    }

    public async Task<SubmissionResponse> GetById(int id)
    {
        string cacheKey = CacheKeys.Submission(id);

        var cachedTask = await _cache.GetTAsync<SubmissionResponse>(cacheKey);
        if (cachedTask != null)
        {
            _logger.LogInformation("Cache HIT - Returning single Submission for ID: {Id}", id);
            return cachedTask;
        }

        _logger.LogInformation("Cache MISS - Fetching Submission from DB for ID: {Id}", id);


        Submission submission = await _context.Submissions.FirstOrDefaultAsync(e => e.Id == id);

        if (submission == null)
        {
            _logger.LogCritical("Submission not found with Id: " + id);

            throw new NotFoundException($"Submission record with Id {id} was not found.");
        }

        _logger.LogInformation("Submission found and retrieved for Id: " + id);

        var response = new SubmissionResponse
        {
            Id = submission.Id,
            Status = submission.Status,
            Notes = submission.Notes,
            SubmissionUrl = submission.SubmissionUrl,
            SubmittedDate = submission.SubmittedDate,
            TaskAssignmentId = submission.TaskAssignmentId
        };

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return response;
    }
}
