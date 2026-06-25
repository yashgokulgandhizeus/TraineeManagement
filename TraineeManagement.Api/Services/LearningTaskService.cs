using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Models;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;
    private readonly ILogger<LearningTaskService> _logger;

    public LearningTaskService(AppDbContext context, ILogger<LearningTaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    private static LearningTask RequestToLearningTask(LearningTaskRequest request)
    {
        return new LearningTask
        {
            Title = request.Title,
            Description = request.Description,
            ExpectedTechStack = request.ExpectedTechStack,
            DueDate = request.DueDate,
            Status = request.Status
        };
    }

    private static LearningTaskResponse LearningTaskToResponse(LearningTask request)
    {
        return new LearningTaskResponse
        {
            Id = request.Id,
            Title = request.Title,
            Desciption = request.Description,
            ExpectedTechStack = request.ExpectedTechStack,
            DueDate = request.DueDate,
            Status = request.Status
        };
    }

    public async Task<LearningTaskResponse> Create(LearningTaskRequest learningTaskRequest)
    {
        LearningTask learningTask = RequestToLearningTask(learningTaskRequest);
        _context.LearningTasks.Add(learningTask);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Learning task created successfully with Title: " + learningTask.Title + " and assigned Id: " + learningTask.Id);

        return LearningTaskToResponse(learningTask);
    }

    public async Task<bool> Delete(int id)
    {
        LearningTask learningTask = await _context.LearningTasks.FirstOrDefaultAsync(e => e.Id == id);

        if (learningTask == null)
        {
            _logger.LogCritical("Failed to delete. Learning task not found with Id: " + id);
            return false;
        }

        _context.LearningTasks.Remove(learningTask);

        await _context.SaveChangesAsync();

        _logger.LogInformation("Learning task with Id: " + id + " deleted successfully.");

        return true;
    }

    public async Task<List<LearningTaskResponse>> GetAll()
    {
        _logger.LogInformation("Fetching all learning tasks from the database.");

        return await _context.LearningTasks
            .Select(e => new LearningTaskResponse
            {
                Id = e.Id,
                Title = e.Title,
                Desciption = e.Description,
                ExpectedTechStack = e.ExpectedTechStack,
                DueDate = e.DueDate,
                Status = e.Status
            })
            .ToListAsync();
    }

    public async Task<LearningTaskResponse> GetById(int id)
    {
        LearningTask l = await _context.LearningTasks.FirstOrDefaultAsync(e => e.Id == id);
        if (l == null)
        {
            _logger.LogCritical("Learning task not found with Id: " + id);
            return null;
        }

        _logger.LogInformation("Learning task found and retrieved for Id: " + id);

        return LearningTaskToResponse(l);
    }

    public async Task<LearningTaskResponse> Update(int id, LearningTaskRequest learningTaskRequest)
    {
        LearningTask learningTask = await _context.LearningTasks.FirstOrDefaultAsync(e => e.Id == id);

        if (learningTask == null)
        {
            _logger.LogCritical("Failed to update. Learning task not found with Id: " + id);
            throw new NotFoundException("not found learning task for requested Id");
        }

        learningTask.Title = learningTaskRequest.Title;
        learningTask.Description = learningTaskRequest.Description;
        learningTask.DueDate = learningTaskRequest.DueDate;
        learningTask.ExpectedTechStack = learningTaskRequest.ExpectedTechStack;
        learningTask.Status = learningTaskRequest.Status;
        learningTask.UpdatedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Learning task with Id: " + id + " updated successfully.");

        return LearningTaskToResponse(learningTask);
    }
}
