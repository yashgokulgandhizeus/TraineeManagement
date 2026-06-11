namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using TraineeManagement.Api.Controllers;

public class TraineeService : ITraineeService
{

    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;

    public TraineeService(AppDbContext context,ILogger<TraineeService> logger)
    {
        _context = context;
        _logger =logger;
    }

    public static TraineeResponse TraineeToResponse(Trainee trainee)
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

    public static Trainee RequestToTrainee(CreateTraineeRequest TraineeRequest)
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

    public async Task<PaginationQueryResponse<TraineeResponse>> GetAll(PaginationQueryRequest Request)
    { 
      IQueryable<Trainee> query= _context.Trainees.AsQueryable();

       if(!string.IsNullOrWhiteSpace(Request.Search))
       {
        query=query.Where(e=>e.FirstName.Contains(Request.Search.Trim()) || e.LastName.Contains(Request.Search.Trim()) || e.Email.Contains(Request.Search.Trim())||e.TechStack.Contains(Request.Search.Trim()));
                                                              
       }
       
       if(!string.IsNullOrEmpty(Request.Status))
        {
        query=query.Where(e=>e.Status.Contains(Request.Status));
                                        
        }

        int skip=(Request.PageNumber-1)*Request.PageSize; 

        _logger.LogInformation("get all request with Page no:"+Request.PageNumber+"Page size:"+Request.PageSize+"Search:"+Request.Search+"and Status:"+Request.Search);

        List<TraineeResponse> Trainees=await query.Select(e=>TraineeToResponse(e))
                                            .Skip(skip)
                                            .Take(Request.PageSize)
                                            .ToListAsync();


                                            
        return new PaginationQueryResponse<TraineeResponse>
        {
            PageSize=Request.PageSize,
            PageNumber=Request.PageNumber,
            TotalRecords=Trainees.Count,
            Data=Trainees
        };
    }

    public async Task<TraineeResponse> GetById(int Id)
    {
        Trainee Trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == Id);

        if (Trainee == null)
        {
            _logger.LogCritical("Trainee not found of ID:"+Id);
            return null;
        }

        _logger.LogInformation("Trainee requested and send as response of ID:"+Id);
        return TraineeToResponse(Trainee);

    }

    public async Task<TraineeResponse> Update(int id, UpdateTraineeRequest Trainee)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
        {
            _logger.LogCritical("Trainee not Found for update of ID:"+id);
            return null;
        }

        else
        {

            t.FirstName = Trainee.FirstName;
            t.LastName = Trainee.LastName;
            t.Email = Trainee.Email;
            t.TechStack = Trainee.TechStack;
            t.Status = Trainee.Status;
            t.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Trainee updated as trainee request with record:"+Trainee.FirstName+","+Trainee.LastName+","+Trainee.Email+","+Trainee.TechStack+","+Trainee.Status);

            return TraineeToResponse(t);
        }
    }

    public async Task<bool> Delete(int Id)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == Id);

        if (t == null)
        {
            _logger.LogCritical("Trainee not Found for delete of ID:"+Id);
            return false;
        }
        else
        {

            _context.Trainees.Remove(t);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Trainee deleted of Id:"+Id);
            return true;

        }
    }

//     public async Task<List<TraineeResponse>> Search(string Search)
//     {

//         List<TraineeResponse> traineeResponses = await _context.Trainees.Where(e => e.FirstName == Search || e.LastName == Search || e.Email == Search || e.TechStack == Search)
//         .Select(t => TraineeToResponse(t)).ToListAsync();

//         if (traineeResponses == null)
//             return null;

//         else
//             return traineeResponses;

// }

}
