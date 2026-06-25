namespace TraineeManagement.Api.Services;


public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;
    public LocalFileStorageService(IConfiguration configuration)
    {
        _rootPath = configuration["FileStorage:UploadPath"]!;
        if(!Directory.Exists(_rootPath)){
            Directory.CreateDirectory(_rootPath);
        }
    }
    public async Task DeleteAsync(string storedFileName)
    {
        string path = Path.Combine(_rootPath , storedFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        await Task.CompletedTask;
        // throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(string storedFileName)
    {
        string path = Path.Combine(_rootPath, storedFileName);

        return await Task.FromResult(File.Exists(path));
        // throw new NotImplementedException();
    }

    public async Task<Stream> OpenReadAsync(string storedFileName)
    {
        string path = Path.Combine(_rootPath , storedFileName);
        Stream stream = new FileStream(path, FileMode.Open , FileAccess.Read);

        return await Task.FromResult(stream);
        // throw new NotImplementedException();
    }

    public Task<string> SaveAsync(Stream fileStream, string extension)
    {
        throw new NotImplementedException();
    }

    public async Task<string> SaveFileAsync(IFormFile file)
    {
        string extension = Path.GetExtension(file.FileName);
        string storedFileName = $"{Guid.NewGuid()}{extension}";
        string fullPath = Path.Combine(_rootPath , storedFileName);

        using var stream = new FileStream(fullPath , FileMode.Create);

        await file.CopyToAsync(stream);
        return storedFileName;
        // throw new NotImplementedException();
    }

   
}
