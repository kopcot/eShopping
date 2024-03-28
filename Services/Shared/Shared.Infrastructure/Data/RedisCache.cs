using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using System.Text;

namespace Shared.Infrastructure.Data
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _expirationTime;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;
        public RedisCache(IDistributedCache cache, TimeSpan expirationTime, IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _expirationTime = expirationTime;
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
        }
        public async Task<(bool, T?)> GetRedisCacheDataAsync<T>(HttpContext context, CancellationToken cancellationToken = default)
        {
            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return (false, default(T));

            var redisJsonValue = await _cache.GetStringAsync(redisKey, cancellationToken);
            if (redisJsonValue == null)
                return (false, default(T));

            T? redisValue = System.Text.Json.JsonSerializer.Deserialize<T>(redisJsonValue);

            //await RefreshRedisCacheData(context, cancellationToken);

            return (redisValue != null, redisValue ?? default);

        }
        public async Task StoreRedisCacheData<T>(HttpContext context, T value, CancellationToken cancellationToken = default)
        {
            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return;

            await Task.Run(async () => 
            {
                string redisJsonValue = System.Text.Json.JsonSerializer.Serialize<T>(value);

                await _cache.SetStringAsync(redisKey,
                    redisJsonValue,
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = _expirationTime
                    });
            }, cancellationToken);
        }
        public async Task RemoveRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default)
        {
            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return;

            await _cache.RemoveAsync(redisKey, cancellationToken);
        }
        public async Task RefreshRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default)
        {
            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return;

            await _cache.RefreshAsync(redisKey, cancellationToken);
        }
        private static string GetRedisKey(HttpContext context) 
        {
            StringBuilder sb = new();
            sb.AppendLine(context.Request.Method);
            sb.AppendLine(context.Request.Path);
            sb.AppendLine(context.Request.QueryString.Value);

            return sb.ToString();
        }
    }
}
