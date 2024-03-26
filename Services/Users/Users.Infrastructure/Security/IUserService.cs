using Shared.Infrastructure.Security;
using Users.Core.Entities;

namespace Users.Infrastructure.Security
{
    public interface IUserService : IBaseSecurity
    {
        Task<string> EncryptPasswordAsync(string? password);
        Task RefreshCode(User user);
        Task<string> HashPasswordAsync(string password, uint code);
        Task<bool> VerifyPassword(string password, string storedHash, uint code);
    }
}
