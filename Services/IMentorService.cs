using TraineeManagement.Api.Dtos;

public interface IMentorService
{
    Task<List<MentorResponse>> GetAll();

    Task<MentorResponse> GetById(int Id);

    Task<MentorResponse> Create(MentorRequest mentorRequest);

    Task<MentorResponse> Update(int Id,MentorRequest mentorRequest);

    Task<bool> Delete(int Id);

}