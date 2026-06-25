using TraineeManagement.Api.Dtos;

public interface IMentorService
{
    Task<List<MentorResponse>> GetAll();

    Task<MentorResponse> GetById(int id);

    Task<MentorResponse> Create(MentorRequest mentorRequest);

    Task<MentorResponse> Update(int id, MentorRequest mentorRequest);

    Task<bool> Delete(int id);

}
