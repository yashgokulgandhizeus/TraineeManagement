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

public class TaskAssignmentService : ITaskAssignmentService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TaskAssignmentService> _logger;
    private readonly IConfiguration _config;
    private readonly ICacheService _cache;

    public TaskAssignmentService(AppDbContext context, ILogger<TaskAssignmentService> logger, IConfiguration config, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _cache = cache;
    }

    public async Task<List<TaskAssignmentResponse>> GetAll()
    {
        string cacheKey = CacheKeys.TaskAssignmentList();

        var cachedData = await _cache.GetTAsync<List<TaskAssignmentResponse>>(cacheKey);

        if (cachedData != null)
        {
            _logger.LogInformation("Cache HIT - Returning Task Assignment.");
            return cachedData;
        }

        _logger.LogInformation("Cache MISS - Querying database.");

        _logger.LogInformation("Fetching all task assignments from the database.");

        var response = await _context.TaskAssignments.Select(e => new TaskAssignmentResponse
        {
            Id = e.Id,
            TraineeId = e.TraineeId,
            TraineeName = $"{e.Trainee.FirstName} {e.Trainee.LastName}",
            MentorId = e.MentorId,
            MentorName = $"{e.Mentor.FirstName} {e.Mentor.LastName}",
            LearningTaskId = e.LearningTaskId,
            TaskTitle = e.LearningTask.Title,
            Status = e.Status,
            AssignedDate = e.AssignedDate,
            DueDate = e.DueDate
        }).ToListAsync();

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return response;
    }

    public async Task<TaskAssignmentResponse> GetById(int id)
    {

        string cacheKey = CacheKeys.TaskAssignment(id);

        var cachedTask = await _cache.GetTAsync<TaskAssignmentResponse>(cacheKey);
        if (cachedTask != null)
        {
            _logger.LogInformation("Cache HIT - Returning single task-assignment for ID: {Id}", id);
            return cachedTask;
        }

        _logger.LogInformation("Cache MISS - Fetching task-assignment from DB for ID: {Id}", id);

        TaskAssignment TaskAssignment = await _context.TaskAssignments
                                                      .Include(a => a.Trainee)
                                                      .Include(a => a.Mentor)
                                                      .Include(a => a.LearningTask)
                                                      .FirstOrDefaultAsync(e => e.Id == id);

        if (TaskAssignment == null)
        {
            _logger.LogCritical("Task assignment not found with Id: " + id);

            throw new NotFoundException($"Task assignment record with Id {id} was not found.");
        }

        _logger.LogInformation("Task assignment found and retrieved for Id: " + id);

        var response = new TaskAssignmentResponse
        {
            Id = TaskAssignment.Id,
            TraineeId = TaskAssignment.TraineeId,
            TraineeName = $"{TaskAssignment.Trainee.FirstName} {TaskAssignment.Trainee.LastName}",
            MentorId = TaskAssignment.MentorId,
            MentorName = $"{TaskAssignment.Mentor.FirstName} {TaskAssignment.Mentor.LastName}",
            LearningTaskId = TaskAssignment.LearningTaskId,
            TaskTitle = TaskAssignment.LearningTask.Title,
            Status = TaskAssignment.Status,
            AssignedDate = TaskAssignment.AssignedDate,
            DueDate = TaskAssignment.DueDate
        };

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return response;
    }

    public async Task<TaskAssignmentResponse> TaskAssignment(TaskAssignmentRequest request)
    {
        var TraineeExists = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == request.TraineeId);
        if (TraineeExists == null)
        {
            _logger.LogCritical("Failed to create task assignment. Trainee not found with Id: " + request.TraineeId);

            throw new NotFoundException($"Trainee profile with Id {request.TraineeId} was not found.");
        }

        var MentorExists = await _context.Mentors.FirstOrDefaultAsync(m => m.Id == request.MentorId);
        if (MentorExists == null)
        {
            _logger.LogCritical("Failed to create task assignment. Mentor not found with Id: " + request.MentorId);

            throw new NotFoundException($"Mentor profile with Id {request.MentorId} was not found.");
        }

        var TaskExists = await _context.LearningTasks.FirstOrDefaultAsync(t => t.Id == request.LearningTaskId);
        if (TaskExists == null)
        {
            _logger.LogCritical("Failed to create task assignment. LearningTask not found with Id: " + request.LearningTaskId);

            throw new NotFoundException($"Learning task definition with Id {request.LearningTaskId} was not found.");
        }

        if (DateTime.UtcNow >= TaskExists.DueDate)
        {
            _logger.LogCritical("Failed to create task assignment. The task due date has already passed or assignment date is invalid.");

            throw new BadRequestException($"Cannot assign task. The learning task deadline ({TaskExists.DueDate}) has already expired.");
        }

        var Assignment = new TaskAssignment
        {
            TraineeId = request.TraineeId,
            MentorId = request.MentorId,
            LearningTaskId = request.LearningTaskId,
            DueDate = TaskExists.DueDate,
            Status = request.Status,
        };

        _context.TaskAssignments.Add(Assignment);
        await _context.SaveChangesAsync();

        var SavedAssignment = await _context.TaskAssignments
            .Include(a => a.Trainee)
            .Include(a => a.Mentor)
            .Include(a => a.LearningTask)
            .FirstAsync(a => a.Id == Assignment.Id);

        _logger.LogInformation("Task assignment created successfully with Id: " + SavedAssignment.Id + " for TraineeId: " + SavedAssignment.TraineeId + " by MentorId: " + SavedAssignment.MentorId);

        var trackedKeys = await _cache.GetTAsync<List<string>>("task-assignment:list:registry");
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
        return new TaskAssignmentResponse
        {
            Id = SavedAssignment.Id,
            TraineeId = SavedAssignment.TraineeId,
            TraineeName = $"{SavedAssignment.Trainee.FirstName} {SavedAssignment.Trainee.LastName}",
            MentorId = SavedAssignment.MentorId,
            MentorName = $"{SavedAssignment.Mentor.FirstName} {SavedAssignment.Mentor.LastName}",
            LearningTaskId = SavedAssignment.LearningTaskId,
            TaskTitle = SavedAssignment.LearningTask.Title,
            AssignedDate = SavedAssignment.AssignedDate,
            Status = SavedAssignment.Status,
            DueDate = SavedAssignment.DueDate
        };
    }

    public async Task<TaskAssignmentResponse> UpdateStatus(int id, AssignmentStatus status)
    {
        // Enums are validated automatically via parameters, custom string checks are no longer needed
        TaskAssignment TaskAssignment = await _context.TaskAssignments
                                                      .Include(a => a.Trainee)
                                                      .Include(a => a.Mentor)
                                                      .Include(a => a.LearningTask)
                                                      .FirstOrDefaultAsync(e => e.Id == id);

        if (TaskAssignment == null)
        {
            _logger.LogCritical("Failed to update status. Task assignment not found with Id: " + id);

            throw new NotFoundException($"Task assignment matrix record with Id {id} was not found.");
        }

        TaskAssignment.Status = status;

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKeys.TaskAssignment(id));

        _logger.LogInformation("Task assignment with Id: " + id + " updated successfully to status: " + status);

        return new TaskAssignmentResponse
        {
            Id = TaskAssignment.Id,
            TraineeId = TaskAssignment.TraineeId,
            TraineeName = $"{TaskAssignment.Trainee.FirstName} {TaskAssignment.Trainee.LastName}",
            MentorId = TaskAssignment.MentorId,
            MentorName = $"{TaskAssignment.Mentor.FirstName} {TaskAssignment.Mentor.LastName}",
            LearningTaskId = TaskAssignment.LearningTaskId,
            TaskTitle = TaskAssignment.LearningTask.Title,
            AssignedDate = TaskAssignment.AssignedDate,
            Status = TaskAssignment.Status,
            DueDate = TaskAssignment.DueDate
        };
    }
}
