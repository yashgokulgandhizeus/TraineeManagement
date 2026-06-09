namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;
using Microsoft.EntityFrameworkCore;

public class TraineeService : ITraineeService
{


    private readonly AppDbContext _context;

    public TraineeService(AppDbContext context)
    {
        _context = context;
    }

    public TraineeResponse TraineeToResponse(Trainee trainee)
    {
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

    public Trainee RequestToTrainee(CreateTraineeRequest TraineeRequest)
    {
        return new Trainee
        {
            FirstName = TraineeRequest.FirstName,
            LastName = TraineeRequest.LastName,
            Email = TraineeRequest.Email,
            TechStack = TraineeRequest.TechStack,
            Status = TraineeRequest.Status,
        };
    }


    public async Task<TraineeResponse> CreateTrainee(CreateTraineeRequest TraineeRequest)
    {

        Trainee trainee = RequestToTrainee(TraineeRequest);

        _context.Trainees.Add(trainee);

        await _context.SaveChangesAsync();

        return TraineeToResponse(trainee);
    }

    public async Task<List<TraineeResponse>> GetAll()
    {
        List<TraineeResponse> Response = await _context.Trainees.Select(t => TraineeToResponse(t)).ToListAsync();

        return Response;
    }

    public async Task<TraineeResponse> GetById(int Id)
    {
        Trainee Trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == Id);

        if (Trainee == null)
        {
            return null;
        }

        return TraineeToResponse(Trainee);

    }

    public async Task<TraineeResponse> Update(int id, UpdateTraineeRequest Trainee)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
            return null;


        else
        {

            t.FirstName = Trainee.FirstName;
            t.LastName = Trainee.LastName;
            t.Email = Trainee.Email;
            t.TechStack = Trainee.TechStack;
            t.Status = Trainee.Status;
            t.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return TraineeToResponse(t);
        }
    }

    public async Task<bool> Delete(int Id)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == Id);

        if (t == null)
            return false;

        else
        {

            _context.Trainees.Remove(t);

            await _context.SaveChangesAsync();
            return true;

        }
    }

    public async Task<List<TraineeResponse>> Search(string Search)
    {

        List<TraineeResponse> traineeResponses = await _context.Trainees.Where(e => e.FirstName == Search || e.LastName == Search || e.Email == Search || e.TechStack == Search)
        .Select(t => TraineeToResponse(t)).ToListAsync();

        if (traineeResponses == null)
            return null;

        else
            return traineeResponses;

}

}
