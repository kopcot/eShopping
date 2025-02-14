using System.Net;
using Users.Core.Entities;

namespace eShopping.Client.Data
{
    public interface IUsersService
    {
        Task<(bool, HttpStatusCode?, Exception?, bool)> GetCheckUserAsync<T>(string username, string password, CancellationToken? cancellationToken = null) where T : User;
        Task<(bool, HttpStatusCode?, Exception?, bool)> IsUserExistsAsync<T>(string username, CancellationToken? cancellationToken = null) where T : User;
        Task<(bool, HttpStatusCode?, Exception?, bool)> CreateUserAsync<T>(string username, string password, string email, CancellationToken? cancellationToken = null) where T : User;
        Task<(bool, HttpStatusCode?, Exception?, bool)> ChangePasswordAsync<T>(string username, string oldPassword, string newPassword, CancellationToken? cancellationToken = null) where T : User;
        Task<(bool, HttpStatusCode?, Exception?, bool)> SendConnectedUserIpAsync<T>(string username, string ipAddress, CancellationToken? cancellationToken = null) where T : IpConnection;
    }
}
