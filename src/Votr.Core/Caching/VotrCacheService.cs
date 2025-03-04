using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Votr.Core.Abstractions.Caching;

namespace Votr.Core.Caching;

public class VotrCacheService(IConnectionMultiplexer connectionMux, ILogger<VotrCacheService> logger) : IVotrCacheService
{

    private readonly IDatabase _database = connectionMux.GetDatabase();

    public Task SetAsAsync<T>(RedisKey key, T value, ushort minutes = 15)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        return SetAsync(key, new RedisValue(jsonValue), minutes);
    }
    public async Task SetAsync(RedisKey key, RedisValue value, ushort minutes)
    {
        if (connectionMux.IsConnected)
        {
            await _database
                .StringSetAsync(key, value, TimeSpan.FromMinutes(minutes))
                .ConfigureAwait(false);
        }
    }

    public async Task<T?> GetAsAsync<T>(RedisKey key)
    {
        var redisValue = await GetAsync(key);
        ConvertToGenericType(redisValue, out T? typedObject);
        return typedObject;
    }
    public async Task<RedisValue> GetAsync(RedisKey key)
    {
        if (connectionMux.IsConnected)
        {
            return await _database.StringGetAsync(key).ConfigureAwait(false);
        }

        return RedisValue.Null;
    }

    public async Task<T> GetOrInitializeAsync<T>(Func<Task<T>> initializeFunction, RedisKey key, ushort timeoutInMinutes = 15)
    {
        try
        {
            if (connectionMux.IsConnected)
            {
                var value = await _database.StringGetAsync(key);
                if (ConvertToGenericType(value, out T? typedObject))
                {
                    if (typedObject != null)
                    {
                        return typedObject;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex,
                "Oops, apparently cache service is unavailable or connection was lost. falling back to the initialization function");
        }

        var initializedObject = await initializeFunction();
        var jsonValue = JsonConvert.SerializeObject(initializedObject);

        if (connectionMux.IsConnected)
        {
            await _database.StringSetAsync(key, jsonValue, TimeSpan.FromMinutes(timeoutInMinutes));
        }

        return initializedObject;
    }

    private static bool ConvertToGenericType<T>(RedisValue value, out T? typedObject)
    {
        typedObject = default;
        if (value.HasValue)
        {
            var deserializedObject = JsonConvert.DeserializeObject<T>(value.ToString());
            if (deserializedObject != null)
            {
                typedObject = deserializedObject;
                return true;
            }
        }

        return false;
    }

    public async Task InvalidateAsync(RedisKey key)
    {
        if (connectionMux.IsConnected)
        {
            await _database
                .KeyDeleteAsync(key)
                .ConfigureAwait(false);
        }
    }

}