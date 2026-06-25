namespace TraineeManagement.Api.Services;

public interface ICacheService
{
    Task<T?> GetTAsync<T>(string key) where T:class;

    Task SetAsync<T>(string key,T value,TimeSpan ttl);

    Task RemoveAsync(string key);

}