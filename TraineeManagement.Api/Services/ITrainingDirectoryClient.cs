namespace TraineeManagement.Api.Services;
using TraineeManagement.Api.Dtos;

public interface ITrainingDirectoryClient
{
    Task<DirectoryProfileDto?> GetProfileAsync(int traineeId, CancellationToken cancellationToken);
}