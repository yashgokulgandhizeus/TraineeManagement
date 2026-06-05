namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;

public class TraineeService : ITraineeService
{
    public TraineeResponse CreateTrainee(CreateTraineeRequest traineeRequest)
    {

        Trainee trainee = new Trainee
        {
            Id = TraineeStore.trainees.Count + 1,
            FirstName = traineeRequest.FirstName,
            LastName = traineeRequest.LastName,
            Email = traineeRequest.Email,
            TechStack = traineeRequest.TechStack,
            Status = traineeRequest.Status,
            CreatedDate = DateTime.Now,
            UpdatedDate = DateTime.Now
        };

        TraineeStore.trainees.Add(trainee);

        return new TraineeResponse
        {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status
        };
    }

    public List<TraineeResponse> GetAll()
    {
        List<TraineeResponse> response = TraineeStore.trainees.Select(t => new TraineeResponse
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            TechStack = t.TechStack,
            Status = t.Status
        }).ToList();

        return response;
    }

    public TraineeResponse GetById(int id)
    {
        Trainee trainee = TraineeStore.trainees.FirstOrDefault(t => t.Id == id);

        if (trainee == null)
        {
            return null;
        }

        return new TraineeResponse
        {
            Id = trainee.Id,
            FirstName = trainee.FirstName,
            LastName = trainee.LastName,
            Email = trainee.Email,
            TechStack = trainee.TechStack,
            Status = trainee.Status
        };

    }

    public TraineeResponse Update(int id, UpdateTraineeRequest trainee)
    {
        Trainee t = TraineeStore.trainees.FirstOrDefault(x => x.Id == id);

        if (t == null)
            return null;


        else
        {

            t.FirstName = trainee.FirstName;
            t.LastName = trainee.LastName;
            t.Email = trainee.Email;
            t.TechStack = trainee.TechStack;
            t.Status = trainee.Status;
            return new TraineeResponse
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                TechStack = t.TechStack,
                Status = t.Status
            };
        }
    }

    public bool Delete(int id)
    {
        Trainee t=TraineeStore.trainees.FirstOrDefault(x=>x.Id==id);

        if(t==null)
        return false;

        else
        {

        TraineeStore.trainees.Remove(t);
        return true;

        }
    }
}