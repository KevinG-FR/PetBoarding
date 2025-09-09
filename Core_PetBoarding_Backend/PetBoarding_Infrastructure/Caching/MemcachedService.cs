using Enyim.Caching;
using Microsoft.Extensions.Logging;
using PetBoarding_Domain.Abstractions;

namespace PetBoarding_Infrastructure.Caching;

public class MemcachedService : ICacheService
{
    private readonly IMemcachedClient _memcachedClient;
    private readonly ILogger<MemcachedService> _logger;

    private readonly TimeSpan DefaultExpirationInMinutes = TimeSpan.FromMinutes(30);

    public MemcachedService(IMemcachedClient memcachedClient, ILogger<MemcachedService> logger)
    {
        _memcachedClient = memcachedClient;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var result = await _memcachedClient.GetAsync<T>(key);
            
            if (result.Success && result.Value != null)
            {
                return result.Value;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache key: {Key}", key);
            return null;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var expirationTime = expiration ?? DefaultExpirationInMinutes;
            var result = await _memcachedClient.SetAsync(key, value, (int)expirationTime.TotalSeconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var expirationTime = expiration ?? DefaultExpirationInMinutes;
            var result = await _memcachedClient.ReplaceAsync(key, value, (int)expirationTime.TotalSeconds);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error replacing cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _memcachedClient.RemoveAsync(key);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {Key}", key);
            return false;
        }
    }

    public async Task<bool> ExistsAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _memcachedClient.GetAsync<T>(key);
            var exists = result.Success && result.Value != null;
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache key existence: {Key}", key);
            return false;
        }
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
        where T : class
    {
        var expirationTime = expiration ?? DefaultExpirationInMinutes;
        
        var cached = await _memcachedClient.GetValueAsync<T>(key);
        if (cached is not null)
        {
            return cached;
        }
        
        var data = await factory();
        
        // Seulement mettre en cache si non-null
        if (data != null)
        {
            await _memcachedClient.SetAsync(key, data, (int)expirationTime.TotalSeconds);
        }       
        
        return data;
    }

    public async Task<bool> FlushAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _memcachedClient.FlushAllAsync();
            _logger.LogInformation("Cache has been flushed successfully");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flushing all cache entries");
            return false;
        }
    }
}