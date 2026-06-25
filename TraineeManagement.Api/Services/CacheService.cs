using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace TraineeManagement.Api.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetTAsync<T>(string key) where T : class
    {
        string? json = await _cache.GetStringAsync(key);

        if (string.IsNullOrWhiteSpace(json))
        {
            _logger.LogInformation("Cache MISS - Key: {Key}", key);
            return null;
        }

        _logger.LogInformation("Cache HIT - Key: {Key}", key);
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        if (value == null) return;

        string json = JsonSerializer.Serialize(value);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        };

        await _cache.SetStringAsync(key, json, options);
        _logger.LogInformation("Cache SET - Key: {Key} with TTL: {Ttl}", key, ttl);
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.RemoveAsync(key);
        _logger.LogInformation("Cache REMOVE - Key: {Key}", key);
    }
}
