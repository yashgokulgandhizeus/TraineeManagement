namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Dtos;

public interface ITraineeService
{
    Task<List<TraineeResponse>> GetAll();

    Task<TraineeResponse> GetById(int id);

    Task<TraineeResponse> CreateTrainee(CreateTraineeRequest traineeRequest);
    Task<TraineeResponse> Update(int id,UpdateTraineeRequest trainee);

    Task<bool> Delete(int id);

    Task<List<TraineeResponse>> Search(string Search);


}