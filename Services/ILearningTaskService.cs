using TraineeManagement.Api.Dtos;

public interface ILearningTaskService
{
    Task<List<LearningTaskResponse>> GetAll();

    Task<LearningTaskResponse> GetById(int Id);

    Task<LearningTaskResponse> Create(LearningTaskRequest Request);

    Task<LearningTaskResponse> Update(int Id,LearningTaskRequest Request);

    Task<bool> Delete(int Id);

}