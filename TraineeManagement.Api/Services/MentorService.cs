using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;
using TraineeManagement.Api.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;
    private readonly ILogger<MentorService> _logger;

    private static Mentor RequestToMentor(MentorRequest request)
    {
        return new Mentor
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Experties = request.Experties,
            Status = request.Status
        };
    }

    private static MentorResponse MentorToResponse(Mentor request)
    {
        return new MentorResponse
        {
            Id = request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Experties = request.Experties,
            Status = request.Status
        };
    }

    public MentorService(AppDbContext context, ILogger<MentorService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<MentorResponse> Create(MentorRequest mentorRequest)
    {
        Mentor mentor = RequestToMentor(mentorRequest);
        _context.Mentors.Add(mentor);

        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor created successfully with Email: " + mentor.Email + " and assigned Id: " + mentor.Id);

        return MentorToResponse(mentor);
    }

    public async Task<bool> Delete(int id)
    {
        Mentor mentor = await _context.Mentors.FirstOrDefaultAsync(e => e.Id == id);

        if (mentor == null)
        {
            _logger.LogCritical("Failed to delete. Mentor not found with Id: " + id);
            throw new NotFoundException($"Mentor with Id {id} was not found.");
        }

        _context.Mentors.Remove(mentor);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Mentor with Id: " + id + " deleted successfully.");
        return true;
    }

    public async Task<List<MentorResponse>> GetAll()
    {
        _logger.LogInformation("Fetching all mentors from the database.");
        
        // EF Core requires explicit property mapping expressions inside Select queries to translate to SQL
        return await _context.Mentors
            .Select(e => new MentorResponse
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Experties = e.Experties,
                Status = e.Status
            })
            .ToListAsync();
    }

    public async Task<MentorResponse> GetById(int id)
    {
        Mentor mentor = await _context.Mentors.FirstOrDefaultAsync(e => e.Id == id);
        if (mentor == null)
        {
            _logger.LogCritical("Mentor not found with Id: " + id);
            throw new NotFoundException($"Mentor with Id {id} was not found.");
        }

        _logger.LogInformation("Mentor found and retrieved for Id: " + id);
        return MentorToResponse(mentor);
    }

    public async Task<MentorResponse> Update(int id, MentorRequest mentorRequest)
    {
        Mentor mentor = await _context.Mentors.FirstOrDefaultAsync(e => e.Id == id);

        if (mentor == null)
        {
            _logger.LogCritical("Failed to update. Mentor not found with Id: " + id);
            throw new NotFoundException($"Mentor with Id {id} was not found.");
        }

        mentor.FirstName = mentorRequest.FirstName;
        mentor.LastName = mentorRequest.LastName;
        mentor.Email = mentorRequest.Email;
        mentor.Experties = mentorRequest.Experties;
        mentor.Status = mentorRequest.Status;
        mentor.UpdatedDate = DateTime.UtcNow; // Updated to UtcNow for database persistence consistency

        await _context.SaveChangesAsync();
        _logger.LogInformation("Mentor with Id: " + id + " updated successfully.");

        return MentorToResponse(mentor);
    }
}
