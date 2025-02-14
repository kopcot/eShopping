using Catalog.Core.Entities;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductTypeRepository : BaseRepository<ProductType>, IProductTypeRepository
    {
        protected new readonly CatalogContext _dbContext;
        public ProductTypeRepository(CatalogContext catalogContext) : base(catalogContext)
        {
            _dbContext = catalogContext;
        }

        public async Task<IEnumerable<ProductType>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(pb => pb.Name == name).ToArrayAsync(cancellationToken);
        }
    }
}
