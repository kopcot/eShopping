using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Shared.Infrastructure.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var authorization))
                return Task.FromResult(AuthenticateResult.NoResult());
            if (!Request.Headers.TryGetValue("API-KEY", out var apiKey))
                return Task.FromResult(AuthenticateResult.NoResult());
            if (!Request.Headers.TryGetValue("userRole", out var role))
                role = "0";
            bool isAuthenticated;
            string username;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(authorization!);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter!)).Split(':');
                isAuthenticated = Authenticate(credentials[0], credentials[1], apiKey!);
                username = credentials[0];
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

            if (!isAuthenticated)
                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role!)
            };
            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            var authenticationTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }

        private static bool Authenticate(string username, string password, string apiKey)
        {
            bool result = username.Length != 0;
            result &= password.Length != 0;
            result &= apiKey.Length != 0;
            result &= apiKey == "ac3as3c0as8689!cas68b6nz1u6lu#";

            return result;
        }
    }
}
