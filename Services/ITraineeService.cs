namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Dtos;

public interface ITraineeService
{
    List<TraineeResponse> GetAll();

    TraineeResponse GetById(int id);

    TraineeResponse CreateTrainee(CreateTraineeRequest traineeRequest);
    TraineeResponse Update(int id,UpdateTraineeRequest trainee);

    bool Delete(int id);
}