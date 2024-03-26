using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Security
{
    public interface IBaseSecurity
    {
        Task<(bool, string?, Exception?)> IsAuthenticatedAsync(HttpRequest request);
        Task<(bool, string?, Exception?)> GetUser(HttpRequest request);
    }
}
