namespace TraineeManagement.Api.Services;

using TraineeManagement.Api.Dtos;

public interface IReviewService
{
    Task<List<ReviewResponse>> GetAll();

    Task<ReviewResponse> GetById(int id);

    Task<ReviewResponse> Create(ReviewRequest reviewRequest);


}
