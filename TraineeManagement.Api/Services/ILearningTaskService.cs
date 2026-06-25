using TraineeManagement.Api.Dtos;

public interface ILearningTaskService
{
    Task<List<LearningTaskResponse>> GetAll();

    Task<LearningTaskResponse> GetById(int id);

    Task<LearningTaskResponse> Create(LearningTaskRequest request);

    Task<LearningTaskResponse> Update(int id, LearningTaskRequest request);

    Task<bool> Delete(int id);

}
