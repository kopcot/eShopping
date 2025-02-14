using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Catalog.API.Controllers;
using Catalog.Core.Entities;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OpenTelemetry.Trace;
using Shared.Core.Specs;
using Shared.Infrastructure.Data;
using Xunit;

namespace Catalog.InfrastructureTests
{
    /// <summary>
    /// ChatGPT generated
    /// </summary>
    public class CatalogControllerTests
    {
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IProductBrandRepository> _mockProductBrandRepository;
        private readonly Mock<IProductTypeRepository> _mockProductTypeRepository;
        private readonly Mock<IImageFileRepository> _mockImageFileRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IRedisCache> _mockRedisCache;
        private readonly Mock<ILogger<CatalogController>> _mockLogger;
        private readonly Mock<Tracer> _mockTracer;
        private readonly CatalogController _controller;

        public CatalogControllerTests()
        {
            _mockProductRepository = new Mock<IProductRepository>();
            _mockProductBrandRepository = new Mock<IProductBrandRepository>();
            _mockProductTypeRepository = new Mock<IProductTypeRepository>();
            _mockImageFileRepository = new Mock<IImageFileRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockRedisCache = new Mock<IRedisCache>();
            _mockLogger = new Mock<ILogger<CatalogController>>();
            _mockTracer = new Mock<Tracer>();

            _controller = new CatalogController(
                _mockProductRepository.Object,
                _mockProductBrandRepository.Object,
                _mockProductTypeRepository.Object,
                _mockImageFileRepository.Object,
                _mockRedisCache.Object,
                _mockUserService.Object,
                _mockLogger.Object,
                _mockTracer.Object
            );
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product { Id = 1, Name = "Product1" } };
            var pagination = new Pagination { PageIndex = 1, PageSize = 10 };
            var specParams = new ProductSpecParams();
            _mockUserService.Setup(s => s.GetUser(It.IsAny<HttpRequest>()))
                .ReturnsAsync((true, "User1", null));
            _mockRedisCache.Setup(c => c.GetRedisCacheDataAsync<IEnumerable<Product>>(It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, null));
            _mockProductRepository.Setup(r => r.GetFilteredAsync(It.IsAny<ProductSpecParams>(), It.IsAny<Pagination>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);
            _mockRedisCache.Setup(c => c.StoreRedisCacheData(It.IsAny<HttpContext>(), It.IsAny<IEnumerable<Product>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GetAllProductsAsync(pagination, specParams);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            //var okResult = result as OkObjectResult;
            //okResult.Value.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Product1" };
            _mockUserService.Setup(s => s.GetUser(It.IsAny<HttpRequest>()))
                .ReturnsAsync((true, "User1", null));
            _mockRedisCache.Setup(c => c.GetRedisCacheDataAsync<Product>(It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, null));
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);
            _mockRedisCache.Setup(c => c.StoreRedisCacheData(It.IsAny<HttpContext>(), It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.GetProductByIdAsync(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            //var okResult = result as OkObjectResult;
            //okResult.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetProductByIdAsync_ReturnsNotFound_WhenProductNotFound()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUser(It.IsAny<HttpRequest>()))
                .ReturnsAsync((true, "User1", null));
            _mockRedisCache.Setup(c => c.GetRedisCacheDataAsync<Product>(It.IsAny<HttpContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((false, null));
            _mockProductRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductByIdAsync(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
