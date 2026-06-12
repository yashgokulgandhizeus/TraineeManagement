using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

public class LearningTaskService : ILearningTaskService
{
    private readonly AppDbContext _context;

        public LearningTaskService(AppDbContext context)
    {
        _context = context;
    }

    public static LearningTask RequestToLearningTask(LearningTaskRequest request)
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

    public static LearningTaskResponse LearningTaskToResponse(LearningTask request)
    {
        return new LearningTaskResponse
        {
            Id=request.Id,
            Title = request.Title,
            Desciption = request.Description,
            ExpectedTechStack = request.ExpectedTechStack,
            DueDate = request.DueDate,
            Status = request.Status
        };
    }


    public async Task<LearningTaskResponse> Create(LearningTaskRequest learningTaskRequest)
    {
        LearningTask learningTask=RequestToLearningTask(learningTaskRequest);
        _context.LearningTasks.Add(learningTask);

        await _context.SaveChangesAsync();

        return LearningTaskToResponse(learningTask);
    }

    public async Task<bool> Delete(int Id)
    {
        LearningTask LearningTask=await _context.LearningTasks.FirstOrDefaultAsync(e=>e.Id==Id);

        if(LearningTask==null)
        return false;

        _context.LearningTasks.Remove(LearningTask);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<LearningTaskResponse>> GetAll()
    {
        return await _context.LearningTasks.Select(e=>LearningTaskToResponse(e))
                                      .ToListAsync();
    
    }

    public async Task<LearningTaskResponse> GetById(int Id)
    {
        LearningTask l=await _context.LearningTasks.FirstOrDefaultAsync(e=>e.Id==Id);
        if(l==null)
        return null;

        return LearningTaskToResponse(l);
    }

    public async Task<LearningTaskResponse> Update(int Id, LearningTaskRequest learningTaskRequest)
    {
        LearningTask learningTask=await _context.LearningTasks.FirstOrDefaultAsync(e=>e.Id==Id);

        if(learningTask==null)
        return null;

        learningTask.Title=learningTaskRequest.Title;
        learningTask.Description=learningTaskRequest.Description;
        learningTask.DueDate=learningTaskRequest.DueDate;
        learningTask.ExpectedTechStack=learningTaskRequest.ExpectedTechStack;
        learningTask.Status=learningTaskRequest.Status;
        learningTask.UpdatedDate=DateTime.Now;

        await _context.SaveChangesAsync();
        
        return LearningTaskToResponse(learningTask);
    }
}