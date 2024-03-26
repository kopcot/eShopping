using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace Shared.Infrastructure.Data
{
    public class RedisCache : IRedisCache
    {
        private readonly IDistributedCache _cache;
        private readonly TimeSpan _expirationTime;
        public RedisCache(IDistributedCache cache, TimeSpan expirationTime)
        {
            _cache = cache;
            _expirationTime = expirationTime;
        }
        public async Task<(bool, T?)> GetRedisCacheDataAsync<T>(HttpContext context, CancellationToken cancellationToken = default)
        {
            var redisJsonValue = await _cache.GetStringAsync(GetRedisKey(context), cancellationToken);
            if (redisJsonValue == null)
                return (false, default(T));

            T? redisValue = System.Text.Json.JsonSerializer.Deserialize<T>(redisJsonValue);

            //await RefreshRedisCacheData(context, cancellationToken);

            return (redisValue != null, redisValue ?? default);

        }
        public async Task StoreRedisCacheData<T>(HttpContext context, T value, CancellationToken cancellationToken = default)
        {
            await Task.Run(async () => 
            {
                string redisJsonValue = System.Text.Json.JsonSerializer.Serialize<T>(value);

                await _cache.SetStringAsync(GetRedisKey(context),
                    redisJsonValue,
                    new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = _expirationTime
                    });
            });
        }
        public async Task RemoveRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default)
        {
            await _cache.RemoveAsync(GetRedisKey(context), cancellationToken);
        }
        public async Task RefreshRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default)
        {
            await _cache.RefreshAsync(GetRedisKey(context), cancellationToken);
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
