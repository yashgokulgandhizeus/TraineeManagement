using Microsoft.EntityFrameworkCore;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos; 
using System.Threading.Tasks;

namespace TraineeManagement.Api.Services;

public class ProcessingJobsService:IProcessingJobsService
{
    private readonly AppDbContext _context;

    public ProcessingJobsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<JobStatusResponse?> GetJobStatus(int id)
    {
        var job = await _context.ProcessingJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return null;
        }

        return new JobStatusResponse
        {
            JobId = job.Id,
            TrackingIdentifier = job.CorrelationId,
            AssociatedFileId = job.FileId,
            CurrentState = job.Status.ToString(),
            ExecutionAttempts = job.Attempts,
            FailureLogs = job.ErrorSummary,
            Timestamps = new
            {
                Initialized = job.CreatedAt,
                ProcessingStarted = job.StartedAt,
                Completed = job.CompletedAt
            }
        };
    }
}
