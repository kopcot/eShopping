using Catalog.Core.Entities;
using Catalog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Repositories;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductBrandRepository : BaseRepository<ProductBrand>, IProductBrandRepository
    {
        protected new readonly CatalogContext _dbContext;
        public ProductBrandRepository(CatalogContext catalogContext) : base(catalogContext)
        {
            _dbContext = catalogContext;
        }

        public async Task<IEnumerable<ProductBrand>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(pb => pb.Name == name).ToListAsync(cancellationToken);
        }
    }
}
