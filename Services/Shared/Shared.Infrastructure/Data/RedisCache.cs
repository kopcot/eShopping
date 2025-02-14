using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry.Trace;
using StackExchange.Redis;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Shared.Infrastructure.Data
{
    public class RedisCache : IRedisCache, IDisposable 
    {
        private readonly IDistributedCache _cache;
        private readonly string _instanceName;
        private readonly TimeSpan _expirationTime;
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly IDatabase _database;
        private readonly Tracer _tracer;
        private readonly TelemetrySpan _telemetrySpan;
        private bool disposedValue;

        public RedisCache(IDistributedCache cache, string instanceName, TimeSpan expirationTime, IConnectionMultiplexer connectionMultiplexer, Tracer tracer)
        {
            _cache = cache;
            _instanceName = instanceName;
            _expirationTime = expirationTime;
            _connectionMultiplexer = connectionMultiplexer;
            _database = _connectionMultiplexer.GetDatabase();
            _tracer = tracer;

            _telemetrySpan = _tracer.StartActiveSpan(nameof(RedisCache));
        }
        public async Task<(bool, T?)> GetRedisCacheDataAsync<T>(HttpContext context, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetRedisCacheDataAsync));

            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return (false, default(T));

            string? redisJsonValue;

            using (var spanPart = _tracer.StartActiveSpan("GetStringAsync"))
            {
                redisJsonValue = await _cache.GetStringAsync(redisKey, cancellationToken);
                if (redisJsonValue == null)
                    return (false, default(T));
            }
            T? redisValue;

            using (var spanPart = _tracer.StartActiveSpan(nameof(System.Text.Json.JsonSerializer.Deserialize)))
            {
                redisValue = System.Text.Json.JsonSerializer.Deserialize<T>(redisJsonValue);
            }
            //await RefreshRedisCacheData(context, cancellationToken);

            return (redisValue != null, redisValue ?? default);

        }
        public async Task StoreRedisCacheData<T>(HttpContext context, T value, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(StoreRedisCacheData));

            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return;

            using (var spanPart = _tracer.StartActiveSpan(nameof(Task.Run)))
            {
                await Task.Run(async () =>
                {
                    string redisJsonValue;
                    using (var spanPart = _tracer.StartActiveSpan(nameof(System.Text.Json.JsonSerializer.Serialize)))
                    {
                        redisJsonValue = System.Text.Json.JsonSerializer.Serialize<T>(value);
                    }

                    using (var spanPart = _tracer.StartActiveSpan("SetStringAsync"))
                    {
                        await _cache.SetStringAsync(redisKey,
                            redisJsonValue,
                            new DistributedCacheEntryOptions()
                            {
                                AbsoluteExpirationRelativeToNow = _expirationTime
                            });
                    };
                }, cancellationToken);
            }
        }
        public async Task RemoveRedisCacheDataAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RemoveRedisCacheDataAsync));

            using (var spanPart = _tracer.StartActiveSpan("RemoveAsync"))
            {
                await _connectionMultiplexer.GetServers().First().FlushDatabaseAsync();
            }
        }
        public async Task RefreshRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RefreshRedisCacheDataAsync));

            var redisKey = GetRedisKey(context);

            if (!_database.IsConnected(redisKey))
                return;

            using (var spanPart = _tracer.StartActiveSpan("RefreshAsync"))
            {
                await _cache.RefreshAsync(redisKey, cancellationToken);
            }
        }

        public async Task FlushDatabaseAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(FlushDatabaseAsync));

            if (!_database.IsConnected(""))
                return;

            var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.Configuration);
            var database = _connectionMultiplexer.GetDatabase();

            using (var spanPart = _tracer.StartActiveSpan("FlushDatabaseAsync_FlushDatabaseAsync"))
            {
                await server.FlushDatabaseAsync(_database.Database);
            }
        }

        private string GetRedisKey(HttpContext context)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetRedisKey));

            StringBuilder sb = new();
            sb.AppendLine(context.Request.Method);
            sb.AppendLine(context.Request.Path);
            sb.AppendLine(context.Request.QueryString.Value);

            return sb.ToString();
        }

        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                _telemetrySpan.Dispose();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~RedisCache()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
