using eShopping.Client.Data;
using eShopping.Client.Pages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Users.Core.Entities;

namespace eShopping.Client.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private IErrorService _errorService;
        private IUsersService _usersService;
        private OpenTelemetry.Trace.Tracer _tracer;
        private IHttpContextAccessor _httpContextAccessor;
        public UserController(IErrorService errorService, IUsersService usersService, OpenTelemetry.Trace.Tracer tracer, IHttpContextAccessor httpContextAccessor)
        {
            _errorService = errorService;
            _usersService = usersService;
            _tracer = tracer;
            _httpContextAccessor = httpContextAccessor;
        }
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Login([FromForm] string name, [FromForm] string password)
        {
            using var span = _tracer.StartActiveSpan($"{nameof(UserController)}_{nameof(Login)}");

            (var result, var httpStatusCode, var exception, var exists) = await _usersService.IsUserExistsAsync<User>(name);
            if (!result)
                await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);
            if (!exists)
            {
                using (var spanSignInAsync = _tracer.StartActiveSpan(nameof(_usersService.SendConnectedUserIpAsync)))
                {
                    var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                    (result, httpStatusCode, exception, var sendIpAddress) = await _usersService.SendConnectedUserIpAsync<IpConnection>(name, remoteIpAddress);
                    await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);
                }

                return Redirect($"/CreateUser");
            }

            (result, httpStatusCode, exception, var accepted) = await _usersService.GetCheckUserAsync<User>(name, password);
            if (!result)
                await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);

            if (accepted)
            {
                using (var spanSignInAsync = _tracer.StartActiveSpan("SignInAsync"))
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreateClaims(name), new AuthenticationProperties()
                    {
                        IssuedUtc = DateTime.UtcNow,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1),
                        AllowRefresh = true
                    });
                };

                using (var spanSignInAsync = _tracer.StartActiveSpan(nameof(_usersService.SendConnectedUserIpAsync)))
                {
                    var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                    (result, httpStatusCode, exception, var sendIpAddress) = await _usersService.SendConnectedUserIpAsync<IpConnection>(name, remoteIpAddress);
                    await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);
                }
            }
            else
                await _errorService.AddSmartErrorAsync(true, string.Empty, null, new Exception("Invalid Username or Password"), null);

            return Redirect("/");
        }
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Create([FromForm] string username, [FromForm] string password, [FromForm] string email)
        {
            using var span = _tracer.StartActiveSpan($"{nameof(UserController)}_{nameof(Create)}");

            (var result, var httpStatusCode, var exception, var createdUser) = await _usersService.CreateUserAsync<User>(username, password, email);
            if (!result)
                await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);

            if (createdUser)
            {
                using (var spanSignInAsync = _tracer.StartActiveSpan("SignInAsync"))
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreateClaims(username), new AuthenticationProperties()
                    {
                        IssuedUtc = DateTime.UtcNow,
                        ExpiresUtc = DateTime.UtcNow.AddDays(1),
                        AllowRefresh = true
                    });
                };

                using (var spanSignInAsync = _tracer.StartActiveSpan(nameof(_usersService.SendConnectedUserIpAsync)))
                {
                    var remoteIpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
                    (result, httpStatusCode, exception, var sendIpAddress) = await _usersService.SendConnectedUserIpAsync<IpConnection>(username, remoteIpAddress);
                    await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);
                }
            }
            else
                await _errorService.AddSmartErrorAsync(true, string.Empty, null, new Exception($"User '{username}' not created"), null);

            return Redirect("/");
        }
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> ChangePassword([FromForm] string oldPassword, [FromForm] string newPassword, [FromForm] string newPasswordRepeat)
        {
            if (!newPasswordRepeat.Equals(newPassword))
            { 
                await _errorService.AddSmartErrorAsync(true, string.Empty, null, new Exception ("New passwords are not matching"), null);
                return Redirect("/ChangePassword");
            }
            string username = HttpContext.User.Claims.First().Value;
            using var span = _tracer.StartActiveSpan($"{nameof(UserController)}_{nameof(ChangePassword)}");
            (var result, var httpStatusCode, var exception, var changedPassword) = await _usersService.ChangePasswordAsync<User>(username, oldPassword, newPassword);
            if (!result)
                await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);

            return Redirect("/");
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Logout()
        {
            using var span = _tracer.StartActiveSpan($"{nameof(UserController)}_{nameof(Logout)}");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
        #region private methods
        private static ClaimsPrincipal CreateClaims(string name)
        {
            ClaimsIdentity claimsIdentity = new(new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, name),
                        new(ClaimTypes.Name, name)
                    }, "auth");
            ClaimsPrincipal claims = new(claimsIdentity);
            return claims;
        }
        #endregion
    }
}
