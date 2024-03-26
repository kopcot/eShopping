using eShopping.Client.Data;
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
        public UserController(IErrorService errorService, IUsersService usersService)
        {
            _errorService = errorService;
            _usersService = usersService;
        }
        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Login([FromForm] string name, [FromForm] string password)
        {
            (var result, var httpStatusCode, var exception, var accepted) = await _usersService.GetCheckUserAsync<User>(name, password);
            if (!result)
                await _errorService.AddSmartErrorAsync(!result, string.Empty, httpStatusCode, exception, null);

            if (accepted)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, CreateClaims(name), new AuthenticationProperties()
                {
                    IssuedUtc = DateTime.UtcNow,
                    ExpiresUtc = DateTime.UtcNow.AddDays(1),
                    AllowRefresh = true
                });
            }
            else
                await _errorService.AddSmartErrorAsync(true, string.Empty, null, new Exception("Invalid Username or Password"), null);

            return Redirect("/");
        }

        [HttpPost]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Logout()
        {
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
