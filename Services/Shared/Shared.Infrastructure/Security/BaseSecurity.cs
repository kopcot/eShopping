using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Shared.Infrastructure.Security
{
    public abstract class BaseSecurity : IBaseSecurity
    {
        public async Task<(bool, string?, Exception?)> GetUser(HttpRequest request)
        {
            try { 
                if (!request.Headers.TryGetValue("Authorization", out var authorization))
                    return await Task.FromResult<(bool, string?, Exception?)>((false, null, new ArgumentException("Authorization")));

                var authHeader = AuthenticationHeaderValue.Parse(authorization!);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter!)).Split(':');
                var username = credentials[0];

                return await Task.FromResult<(bool, string?, Exception?)>((true, username, null));
            }
            catch (Exception ex)
            {
                return await Task.FromResult<(bool, string?, Exception?)>((false, null, ex));
            }

        }
        #region Authentication
        public async Task<(bool, string?, Exception?)> IsAuthenticatedAsync(HttpRequest request)
        {
            var authenticateResult = await request.HttpContext.AuthenticateAsync();
            var role = authenticateResult?.Ticket?.Principal?.Claims?.FirstOrDefault(defaultValue => defaultValue.Type == ClaimTypes.Role);  
            return await Task.FromResult(
                (
                    authenticateResult?.Succeeded ?? false,
                    authenticateResult?.Ticket?.Principal?.Identity?.Name ?? string.Empty,
                    authenticateResult?.Failure ?? null
                ));
        }
        #endregion
    }
}
