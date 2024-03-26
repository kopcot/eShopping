using System.Net;
using Users.Core.Entities;

namespace eShopping.Client.Data
{
    public interface IUsersService
    {
        Task<(bool, HttpStatusCode?, Exception?, bool)> GetCheckUserAsync<T>(string username, string password, CancellationToken? cancellationToken = null) where T : User;
    }
}
