using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace Catalog.InfrastructureTests
{
    public class CatalogContextTests : IDisposable
    {
        private readonly CatalogContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IProductBrandRepository _productBrandRepository;
        private readonly IImageFileRepository _ImageFileRepository;
        private readonly IUserService _userService;
        public CatalogContextTests()
        {
            IUserService userService = new UserService();

            var options = new DbContextOptionsBuilder<CatalogContext>()
                .UseMySQL("Server=192.168.1.200;Port=3307;Database=EShopping;User=user;Password=E1Z/0UbU7*FZq/A.;")
                .Options;
            _context = new CatalogContext(options);
            //_context.Database.EnsureDeleted();
            _context.Database.Migrate();

            _productRepository = new ProductRepository(_context);
            _productTypeRepository = new ProductTypeRepository(_context);
            _productBrandRepository = new ProductBrandRepository(_context);
            _ImageFileRepository = new ImageFileRepository(_context);
            _userService = new UserService();
        }
        public void Dispose()
        {
            //_context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Fact]
        public async Task StartTest_ReturnAllwaysTrue()
        {
            await Task.CompletedTask;

            Assert.True(true);
        }
        [Fact]
        public async Task CanConnect_ReturnTrue()
        {
            bool canConect = await _productRepository.CanConnectAsync();
            canConect &= await _productTypeRepository.CanConnectAsync();
            canConect &= await _productBrandRepository.CanConnectAsync();
            canConect &= await _ImageFileRepository.CanConnectAsync();

            Assert.True(canConect);
        }
    }
}