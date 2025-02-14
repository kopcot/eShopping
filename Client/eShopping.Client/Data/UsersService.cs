using eShopping.Client.Components.ShoppingCart;
using Microsoft.AspNetCore.WebUtilities;
using OpenTelemetry.Trace;
using System.Diagnostics;
using System.Net;
using Users.Core.Entities;

namespace eShopping.Client.Data
{
    public class UsersService : BaseHttpClientService<UsersService>, IUsersService
    {
        public UsersService(string? addressIP, string? routeAPI, HttpClient httpClient, ILogger<UsersService> logger, IHttpContextAccessor httpContextAccessor, Tracer tracer)
            : base(addressIP, routeAPI, httpClient, logger, httpContextAccessor, tracer)
        {
        }
        public async Task<(bool, HttpStatusCode?, Exception?, bool)> GetCheckUserAsync<T>(string username, string password, CancellationToken? cancellationToken) where T : User
        {
            using var span = _tracer.StartActiveSpan(nameof(GetCheckUserAsync));

            T? user = new User() { Name = username, Password = password } as T;
            return await _httpResponseResolver.ResolvePostAsJsonResponse<T, bool>(_routeAPI + typeof(T).Name + "/check", user!, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, bool)> IsUserExistsAsync<T>(string username, CancellationToken? cancellationToken) where T : User
        {
            using var span = _tracer.StartActiveSpan(nameof(IsUserExistsAsync));

            return await _httpResponseResolver.ResolveGetResponse<bool>(_routeAPI + typeof(T).Name + $"/exists/{username}", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, bool)> CreateUserAsync<T>(string username, string password, string email, CancellationToken? cancellationToken) where T : User
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateUserAsync));

            T? user = new User()
            {
                Name = username,
                Password = password,
                Email = email,
                Role = Users.Core.Enums.UserRole.RoleType.User
            } as T;
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T, bool>(_routeAPI + typeof(T).Name, user!, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, bool)> ChangePasswordAsync<T>(string username, string oldPassword, string newPassword, CancellationToken? cancellationToken = null) where T : User
        {
            using var span = _tracer.StartActiveSpan(nameof(ChangePasswordAsync));

            T? user = new User()
            {
                Name = username,
                Password = newPassword
            } as T;

            Dictionary<string, string?> queryParam = [];
            queryParam.Add("oldPassword", oldPassword);

            var queryString = QueryHelpers.AddQueryString("", queryParam);

            return await _httpResponseResolver.ResolvePostAsJsonResponse<T, bool>(_routeAPI + typeof(T).Name + "/updatepassword" + queryString, user!, cancellationToken);
        }

        public async Task<(bool, HttpStatusCode?, Exception?, bool)> SendConnectedUserIpAsync<T>(string username, string ipAddress, CancellationToken? cancellationToken = null) where T : IpConnection
        {
            using var span = _tracer.StartActiveSpan(nameof(SendConnectedUserIpAsync));

            Dictionary<string, string?> queryParam = [];
            queryParam.Add("username", username);
            queryParam.Add("ipAddress", ipAddress);
            
            var queryString = QueryHelpers.AddQueryString("", queryParam);

            return await _httpResponseResolver.ResolvePutResponse<bool>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
    }
}
