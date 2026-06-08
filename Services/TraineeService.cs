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

    static int id=1;

    public async Task<TraineeResponse> CreateTrainee(CreateTraineeRequest traineeRequest)
    {
        
        Trainee trainee = new Trainee
        {
            Id = id++,
            FirstName = traineeRequest.FirstName,
            LastName = traineeRequest.LastName,
            Email = traineeRequest.Email,
            TechStack = traineeRequest.TechStack,
            Status = traineeRequest.Status,

        };

        _context.Trainees.Add(trainee);

        await _context.SaveChangesAsync();

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

    public async Task<List<TraineeResponse>> GetAll()
    {
        List<TraineeResponse> response =await _context.Trainees.Select(t => new TraineeResponse
        {
            Id = t.Id,
            FirstName = t.FirstName,
            LastName = t.LastName,
            Email = t.Email,
            TechStack = t.TechStack,
            Status = t.Status
        }).ToListAsync();

        return response;
    }

    public async Task<TraineeResponse> GetById(int id)
    {
        Trainee trainee =await _context.Trainees.FirstOrDefaultAsync(t => t.Id == id);

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

    public async Task<TraineeResponse> Update(int id, UpdateTraineeRequest trainee)
    {
        Trainee t =await _context.Trainees.FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
            return null;


        else
        {

            t.FirstName = trainee.FirstName;
            t.LastName = trainee.LastName;
            t.Email = trainee.Email;
            t.TechStack = trainee.TechStack;
            t.Status = trainee.Status;
            t.UpdatedDate=DateTime.Now;
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

    public async Task<bool> Delete(int id)
    {
        Trainee t=await _context.Trainees.FirstOrDefaultAsync(x=>x.Id==id);

        if(t==null)
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
        
        List<TraineeResponse> traineeResponses=await _context.Trainees.Where(e=>e.FirstName==Search || e.LastName==Search || e.Email==Search || e.TechStack==Search).Select(t=>new TraineeResponse{
            Id=t.Id,
            FirstName=t.FirstName,
            LastName=t.LastName,
            Email=t.Email,
            TechStack=t.TechStack,
            Status=t.Status
        }).ToListAsync();

        if(traineeResponses==null)
        return null;

        else
        return traineeResponses;
        

    }

 
}
