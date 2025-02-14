using Shared.Infrastructure.Repositories;
using Users.Core.Entities;

namespace Users.Infrastructure.Repositories
{
    public interface IIpConnectionRepository : IAsyncRepository<IpConnection>
    {
    }
}
