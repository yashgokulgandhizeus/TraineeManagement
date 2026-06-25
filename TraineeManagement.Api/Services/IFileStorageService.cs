using System.IO;
using System.Threading.Tasks;
namespace TraineeManagement.Api.Services;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream, string extension);
    Task<Stream> OpenReadAsync(string storageName);
    Task<bool> ExistsAsync(string storageName);
    Task DeleteAsync(string storageName);
    Task<string> SaveFileAsync(IFormFile file);
}
