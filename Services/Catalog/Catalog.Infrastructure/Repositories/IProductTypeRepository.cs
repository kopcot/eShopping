using Catalog.Core.Entities;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public interface IProductTypeRepository : IAsyncRepository<ProductType>
    {
        Task<IEnumerable<ProductType>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
