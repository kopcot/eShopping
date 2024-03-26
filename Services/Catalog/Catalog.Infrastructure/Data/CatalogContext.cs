using Catalog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Data;

namespace Catalog.Infrastructure.Data
{
    public class CatalogContext : BaseDbContext
    {
        public static DateTime StartTimeStamp { get; set; } = DateTime.UtcNow;
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ImageFileDirectory> ImageFileDirectories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
