using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Infrastructure.Data
{
    public interface IRedisCache
    {
        Task<(bool, T?)> GetRedisCacheDataAsync<T>(HttpContext context, CancellationToken cancellationToken = default); 
        Task StoreRedisCacheData<T>(HttpContext context, T value, CancellationToken cancellationToken = default);
        Task RemoveRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default);
        Task RefreshRedisCacheDataAsync(HttpContext context, CancellationToken cancellationToken = default);
    }
}
