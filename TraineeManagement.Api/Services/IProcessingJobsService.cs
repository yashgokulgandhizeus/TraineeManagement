using TraineeManagement.Api.Dtos;

namespace TraineeManagement.Api.Services;

public interface IProcessingJobsService
{
     public Task<JobStatusResponse?> GetJobStatus(int id);
}