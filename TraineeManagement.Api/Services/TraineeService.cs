namespace TraineeManagement.Api.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Exceptions;

public class TraineeService : ITraineeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<TraineeService> _logger;
    private readonly IConfiguration _config;
    private readonly ICacheService _cache;

    public TraineeService(AppDbContext context, ILogger<TraineeService> logger, IConfiguration config, ICacheService cache)
    {
        _context = context;
        _logger = logger;
        _config = config;
        _cache = cache;
    }

    private static TraineeResponse TraineeToResponse(Trainee trainee)
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

    private static Trainee RequestToTrainee(CreateTraineeRequest traineeRequest)
    {
        return new Trainee
        {
            FirstName = traineeRequest.FirstName,
            LastName = traineeRequest.LastName,
            Email = traineeRequest.Email,
            TechStack = traineeRequest.TechStack,
            Status = traineeRequest.Status,
        };
    }

    public async Task<TraineeResponse> CreateTrainee(CreateTraineeRequest traineeRequest)
    {
        Trainee trainee = RequestToTrainee(traineeRequest);

        _context.Trainees.Add(trainee);
        await _context.SaveChangesAsync();

        var trackedKeys = await _cache.GetTAsync<List<string>>("trainee:list:registry");
        if (trackedKeys != null && trackedKeys.Count > 0)
        {
            foreach (var key in trackedKeys)
            {
                // Strict Safety Check: Only physically delete keys belonging to lists
                if (key.Contains(":list:"))
                {
                    await _cache.RemoveAsync(key);
                }
            }
            _logger.LogInformation("Physically cleared {Count} trainee list keys from cache.", trackedKeys.Count);
        }

        return TraineeToResponse(trainee);
    }

    public async Task<PaginationQueryResponse<TraineeResponse>> GetAll(PaginationQueryRequest request)
    {
        string cacheKey = CacheKeys.TraineeList(request.PageNumber, request.PageSize, request.Search, request.Status?.ToString() ?? "all");

        var cachedData = await _cache.GetTAsync<PaginationQueryResponse<TraineeResponse>>(cacheKey);

        if (cachedData != null)
        {
            _logger.LogInformation("Cache HIT - Returning paginated trainees list.");
            return cachedData;
        }

        _logger.LogInformation("Cache MISS - Querying database.");

        IQueryable<Trainee> query = _context.Trainees.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerms = request.Search.Trim();
            query = query.Where(e => e.FirstName.Contains(searchTerms) ||
                                     e.LastName.Contains(searchTerms) ||
                                     e.Email.Contains(searchTerms) ||
                                     e.TechStack.Contains(searchTerms));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(e => e.Status == request.Status.Value);
        }

        int totalRecords = await query.CountAsync();
        int skip = (request.PageNumber - 1) * request.PageSize;

        _logger.LogInformation("Get all request with Page no: " + request.PageNumber + ", Page size: " + request.PageSize + ", Search: " + request.Search + " and Status: " + request.Status);

        // Fixed Projection: Converted custom mapping function to an explicit object expression lambda matching SQL compilation rules
        List<TraineeResponse> trainees = await query
                                            .Select(e => new TraineeResponse
                                            {
                                                Id = e.Id,
                                                FirstName = e.FirstName,
                                                LastName = e.LastName,
                                                Email = e.Email,
                                                TechStack = e.TechStack,
                                                Status = e.Status
                                            })
                                            .Skip(skip)
                                            .Take(request.PageSize)
                                            .ToListAsync();

        var response = new PaginationQueryResponse<TraineeResponse>
        {
            PageSize = request.PageSize,
            PageNumber = request.PageNumber,
            TotalRecords = totalRecords,
            Data = trainees
        };

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return response;
    }

    public async Task<TraineeResponse> GetById(int id)
    {
        string cacheKey = CacheKeys.Trainee(id);

        var cachedTrainee = await _cache.GetTAsync<TraineeResponse>(cacheKey);
        if (cachedTrainee != null)
        {
            _logger.LogInformation("Cache HIT - Returning single trainee profile for ID: {Id}", id);
            return cachedTrainee;
        }

        _logger.LogInformation("Cache MISS - Fetching trainee profile from DB for ID: {Id}", id);

        Trainee trainee = await _context.Trainees.FirstOrDefaultAsync(t => t.Id == id);

        if (trainee == null)
        {
            _logger.LogCritical("Trainee not found of ID: " + id);
            throw new NotFoundException($"Trainee record with Id {id} was not found.");
        }

        _logger.LogInformation("Trainee requested and sent as response of ID: " + id);

        var response = TraineeToResponse(trainee);

        await _cache.SetAsync(cacheKey, response, TimeSpan.FromMinutes(_config.GetValue<int>("ConnectionStrings:CacheMinutes")));

        return TraineeToResponse(trainee);
    }

    public async Task<TraineeResponse> Update(int id, UpdateTraineeRequest trainee)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
        {
            _logger.LogCritical("Trainee not Found for update of ID: " + id);
            throw new NotFoundException($"Trainee profile with Id {id} was not found for modification update parameters.");
        }

        t.FirstName = trainee.FirstName;
        t.LastName = trainee.LastName;
        t.Email = trainee.Email;
        t.TechStack = trainee.TechStack;
        t.Status = trainee.Status;
        t.UpdatedDate = DateTime.UtcNow; // Updated to UtcNow

        await _context.SaveChangesAsync();

        await _cache.RemoveAsync(CacheKeys.Trainee(id));

        _logger.LogInformation("Trainee updated as trainee request with record: " + trainee.FirstName + "," + trainee.LastName + "," + trainee.Email + "," + trainee.TechStack + "," + trainee.Status);

        return TraineeToResponse(t);
    }

    public async Task<bool> Delete(int id)
    {
        Trainee t = await _context.Trainees.FirstOrDefaultAsync(x => x.Id == id);

        if (t == null)
        {
            _logger.LogCritical("Trainee not Found for delete of ID: " + id);
            throw new NotFoundException($"Trainee profile with Id {id} was not found for removal deletion targets.");
        }

        _context.Trainees.Remove(t);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Trainee deleted of Id: " + id);

        await _cache.RemoveAsync(CacheKeys.Trainee(id));

        return true;
    }
}
