using Shared.Infrastructure.Security;
using System.Security.Cryptography;
using System.Text;
using Users.Core.Entities;

namespace Users.Infrastructure.Security
{
    public class UserService : BaseSecurity, IUserService
    {
        public async Task<string> EncryptPasswordAsync(string? password)
        {
            if (password is null)
                return await Task.FromResult(string.Empty);

            StringBuilder sBuilder = new();

            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
            }

            return await Task.FromResult(sBuilder.ToString());
        }
        public async Task RefreshCode(User user)
        {
            user.Code = (uint)new Random().Next(0, 9999);
            await Task.CompletedTask;
        }
        public async Task<string> HashPasswordAsync(string password, uint code)
        {
            // Generate a salt
            var saltBytes = Encoding.UTF8.GetBytes(code.ToString());
            string salt = Convert.ToBase64String(saltBytes);

            // Hash the password with the salt
            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = SHA256.HashData(combinedBytes);
            string hash = Convert.ToBase64String(hashBytes);

            return await Task.FromResult(hash);
        }
        public async Task<bool> VerifyPassword(string password, string storedHash, uint code)
        {
            // Generate a salt
            var saltBytes = Encoding.UTF8.GetBytes(code.ToString());
            string salt = Convert.ToBase64String(saltBytes);

            byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = SHA256.HashData(combinedBytes);
            string hash = Convert.ToBase64String(hashBytes);

            return await Task.FromResult(hash == storedHash);
        }
    }
}
