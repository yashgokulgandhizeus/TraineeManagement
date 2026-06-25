using TraineeManagement.Api.Dtos;
using TraineeManagement.Api.Models;

namespace TraineeManagement.Api.Services;

public interface ISubmissionFileService
{
    Task<UploadFileResponse?> UploadAsync(int submissionId , IFormFile file);
    Task<(Stream stream , string contentType , string fileName)?> DownloadAsync(int fileId);
    Task<bool> DeleteAsync(int fileId);
}