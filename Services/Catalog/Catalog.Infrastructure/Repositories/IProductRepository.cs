using Catalog.Core.Entities;
using Catalog.Core.Specs;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public interface IProductRepository : IAsyncRepository<Product>
    {
        Task<long> GetCountAsync(ProductSpecParams? catalogSpecParam = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetFilteredAsync(ProductSpecParams catalogSpecParam, Pagination? pagination, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByBrandNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Product?> GetByBrandIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetByTypeNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Product?> GetByTypeIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
