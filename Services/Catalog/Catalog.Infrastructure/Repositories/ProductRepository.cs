using Catalog.Core.Entities;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Specs;
using Shared.Infrastructure.Repositories;
using Shared.Infrastructure.Extensions;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        protected new readonly CatalogContext _dbContext;
        public ProductRepository(CatalogContext catalogContext) : base(catalogContext)
        {
            _dbContext = catalogContext;
        }
        public new async Task<bool> AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductBrands.Attach(entity.Brand);
            _dbContext.ProductTypes.Attach(entity.Type);
            if (entity.ImageFileDirectory != null) 
                _dbContext.ImageFileDirectories.Attach(entity.ImageFileDirectory);

            await _dbSet.AddAsync(entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public new async Task<bool> AddRangeAsync(IEnumerable<Product> entities, CancellationToken cancellationToken = default)
        {
            var brands = entities.GroupBy(e => e.Brand).Select(e => e.Key);
            _dbContext.ProductBrands.AttachRange(brands);
            var types = entities.GroupBy(e => e.Type).Select(e => e.Key);
            _dbContext.ProductTypes.AttachRange(types);
            var imageFileDirectory = entities.GroupBy(e => e.ImageFileDirectory).Select(e => e.Key).Where(e => e != null);
            if (imageFileDirectory.Any())
                _dbContext.ImageFileDirectories.AttachRange(imageFileDirectory!);

            await _dbSet.AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        public new async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .Where(p => p.Name == name)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByBrandNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .Where(p => p.Brand.Name == name)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }
        public async Task<Product?> GetByBrandIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .SingleOrDefaultAsync(p => p.BrandID == id, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetByTypeNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .Where(p => p.Type.Name == name)
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<Product?> GetByTypeIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .SingleOrDefaultAsync(p => p.TypeID == id, cancellationToken);
        }

        public new async Task<IEnumerable<Product>> GetAllAsync(Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .UsePagination(pagination)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<long> GetCountAsync(ProductSpecParams? catalogSpecParam, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Filter(catalogSpecParam)
                .AsNoTracking()
                .LongCountAsync(cancellationToken);
        }
        public async Task<IEnumerable<Product>> GetFilteredAsync(ProductSpecParams catalogSpecParam, Pagination? pagination = null, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(entity => !entity.IsDeleted)
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Include(p => p.ImageFileDirectory)
                .Filter(catalogSpecParam)
                .Sort(catalogSpecParam?.Sorting)
                .UsePagination(pagination)
                .AsNoTracking()
                //.AsSplitQuery()
                .ToArrayAsync(cancellationToken);
        }
    }
}
