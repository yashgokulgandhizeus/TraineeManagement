namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Dtos;

public interface ISubmissionService
{
   Task<SubmissionResponse> Create(SubmissionRequest request);

   Task<List<SubmissionResponse>> GetAll();

   Task<SubmissionResponse> GetById(int id);

}