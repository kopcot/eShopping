using Users.Core.Entities;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;
using System.Threading;

namespace Users.Infrastructure.Repositories
{
    public interface IUserRepository : IAsyncRepository<User>
    {
        Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<bool> IsNameExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
