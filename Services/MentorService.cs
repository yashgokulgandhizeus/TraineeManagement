using Microsoft.EntityFrameworkCore;
using Namotion.Reflection;
using TraineeManagement.Api.Data;
using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

public class MentorService : IMentorService
{
    private readonly AppDbContext _context;

    public static Mentor RequestToMentor(MentorRequest request)
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

    public static MentorResponse MentorToResponse(Mentor request)
    {
        return new MentorResponse
        {
            Id=request.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Experties = request.Experties,
            Status = request.Status
        };
    }

    public MentorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MentorResponse> Create(MentorRequest mentorRequest)
    {
        Mentor mentor=RequestToMentor(mentorRequest);
        _context.Mentors.Add(mentor);

        await _context.SaveChangesAsync();

        return MentorToResponse(mentor);
    }

    public async Task<bool> Delete(int Id)
    {
        Mentor mentor=await _context.Mentors.FirstOrDefaultAsync(e=>e.Id==Id);

        if(mentor==null)
        return false;

        _context.Mentors.Remove(mentor);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<MentorResponse>> GetAll()
    {
        return await _context.Mentors.Select(e=>MentorToResponse(e))
                                      .ToListAsync();
    
    }

    public async Task<MentorResponse> GetById(int Id)
    {
        Mentor mentor=await _context.Mentors.FirstOrDefaultAsync(e=>e.Id==Id);
        if(mentor==null)
        return null;
        
        return MentorToResponse(mentor);
    }

    public async Task<MentorResponse> Update(int Id, MentorRequest mentorRequest)
    {
        Mentor mentor=await _context.Mentors.FirstOrDefaultAsync(e=>e.Id==Id);

        if(mentor==null)
        return null;

        mentor.FirstName=mentorRequest.FirstName;
        mentor.LastName=mentorRequest.LastName;
        mentor.Email=mentorRequest.Email;
        mentor.Experties=mentorRequest.Experties;
        mentor.Status=mentorRequest.Status;
        mentor.UpdatedDate=DateTime.Now;

        await _context.SaveChangesAsync();
        
        return MentorToResponse(mentor);
    }
}