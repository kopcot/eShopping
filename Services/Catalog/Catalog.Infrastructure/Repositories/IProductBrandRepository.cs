using Catalog.Core.Entities;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public interface IProductBrandRepository : IAsyncRepository<ProductBrand>
    {
        Task<IEnumerable<ProductBrand>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
