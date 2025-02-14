using Catalog.Core.Entities;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using OpenTelemetry.Trace;
using Shared.Api.Controllers;
using Shared.Core.Specs;
using Shared.Infrastructure.Data;
using System.Net;

namespace Catalog.API.Controllers
{
    public class CatalogController : ApiAuthorizeController
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductBrandRepository _productBrandRepository;
        private readonly IProductTypeRepository _productTypeRepository;
        private readonly IImageFileRepository _imageFileRepository;
        private readonly IUserService _userService;
        private readonly IRedisCache _redisCache;
        public CatalogController(IProductRepository productRepository,
            IProductBrandRepository productBrandRepository,
            IProductTypeRepository productTypeRepository,
            IImageFileRepository imageFileRepository,
            IRedisCache redisCache,
            IUserService userService,
            ILogger<CatalogController> logger,
            Tracer tracer) : base(userService, logger, tracer)
        {
            _productRepository = productRepository;
            _productBrandRepository = productBrandRepository;
            _productTypeRepository = productTypeRepository;
            _imageFileRepository = imageFileRepository;
            _userService = userService;
            _redisCache = redisCache;
        }
        #region Product
        [HttpGet]
        [Route(nameof(Product))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetAllProductsAsync([FromQuery] Pagination? pagination, [FromQuery] ProductSpecParams catalogSpecParam, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllProductsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var products) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<Product>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(products);

            products = await _productRepository.GetFilteredAsync(catalogSpecParam, pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {products.Count()} products");

            await _redisCache.StoreRedisCacheData(HttpContext, products, cancellationToken);
            return Ok(products);
        }
        [HttpGet]
        [Route(nameof(Product) + "/{id}")]
        //[Route(nameof(Product) + "/[action]/{id}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Product?>> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductByIdAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var product) = await _redisCache.GetRedisCacheDataAsync<Product>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(product);

            _logger.LogInformation($"User = {user}. Return product with id = {id}");
            product = await _productRepository.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Return product exists: {product is not null}");

            await _redisCache.StoreRedisCacheData(HttpContext, product, cancellationToken);
            return product is null ? NotFound() : Ok(product);
        }
        [HttpGet]
        [Route(nameof(Product) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetProductsCountAsync([FromQuery] ProductSpecParams catalogSpecParam, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductsCountAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(count);

            count = await _productRepository.GetCountAsync(catalogSpecParam, cancellationToken);
            _logger.LogInformation($"User = {user}. Get product count");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(count);
        }
        [HttpPut]
        [Route(nameof(Product))]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<Product>> CreateProductAsync(Product product, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var created = await _productRepository.AddAsync(product, cancellationToken);
            _logger.LogInformation($"User = {user}. Create product, id : {product.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, product, cancellationToken);
            return created ? Created(string.Empty, product) : BadRequest();
        }
        [HttpDelete]
        [Route(nameof(Product) + "/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<bool>> RemoveProductTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RemoveProductTypeAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var deleted = await _productRepository.DeleteByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Deleted product, id : {id}");

            await _redisCache.RemoveRedisCacheDataAsync(cancellationToken);
            return deleted ? Ok(deleted) : NotFound();
        }
        #endregion

        #region ProductBrand
        [HttpGet]
        [Route(nameof(ProductBrand))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ProductBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductBrand>>> GetAllProductBrandsAsync([FromQuery] Pagination? pagination, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllProductBrandsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var productBrands) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ProductBrand>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(productBrands);

            productBrands = await _productBrandRepository.GetAllAsync(pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {productBrands.Count()} product brands");

            await _redisCache.StoreRedisCacheData(HttpContext, productBrands, cancellationToken);
            return Ok(productBrands);
        }
        [HttpGet]
        [Route(nameof(ProductBrand) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetProductBrandCountAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductBrandCountAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(count);

            count = await _productBrandRepository.GetCountAsync(cancellationToken);
            _logger.LogInformation($"User = {user}. Get product brand count");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(count);
        }

        [HttpPut]
        [Route(nameof(ProductBrand))]
        [ProducesResponseType(typeof(ProductBrand), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ProductBrand>> CreateProductBrandAsync(ProductBrand productBrand, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductBrandAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var existing = await _productBrandRepository.GetByNameAsync(productBrand.Name, cancellationToken);
            if (existing.Any())
                return StatusCode((int)HttpStatusCode.NotModified, "Already existing");

            var created = await _productBrandRepository.AddAsync(productBrand, cancellationToken);
            _logger.LogInformation($"User = {user}. Create product brand, id : {productBrand.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, productBrand, cancellationToken);
            return created ? Created(string.Empty, productBrand) : BadRequest();
        }
        #endregion

        #region ProductType
        [HttpGet]
        [Route(nameof(ProductType))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ProductType>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ProductType>>> GetAllProductTypesAsync([FromQuery] Pagination? pagination, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllProductTypesAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var productTypes) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ProductType>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(productTypes);

            productTypes = await _productTypeRepository.GetAllAsync(pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {productTypes.Count()} product types");

            await _redisCache.StoreRedisCacheData(HttpContext, productTypes, cancellationToken);
            return Ok(productTypes);
        }
        [HttpGet]
        [Route(nameof(ProductType) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetProductTypeCountAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductTypeCountAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(count);

            count = await _productTypeRepository.GetCountAsync(cancellationToken);
            _logger.LogInformation($"User = {user}. Get product type count");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(count);
        }

        [HttpPut]
        [Route(nameof(ProductType))]
        [ProducesResponseType(typeof(ProductType), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ProductType>> CreateProductTypeAsync(ProductType productType, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductTypeAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var existing = await _productTypeRepository.GetByNameAsync(productType.Name, cancellationToken);
            if (existing.Any())
                return StatusCode((int)HttpStatusCode.NotModified, "Already existing");

            var created = await _productTypeRepository.AddAsync(productType, cancellationToken);
            _logger.LogInformation($"User = {user}. Create product type, id : {productType.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, productType, cancellationToken);
            return created ? Created(string.Empty, productType) : BadRequest();
        }
        #endregion
        #region Image directories
        [HttpGet]
        [Route(nameof(ImageFileDirectory))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ImageFileDirectory>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ImageFileDirectory>>> GetAllImageFileDirectoriesAsync([FromQuery] Pagination? pagination, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllImageFileDirectoriesAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var imageFileDirectories) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ImageFileDirectory>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(imageFileDirectories);

            imageFileDirectories = await _imageFileRepository.GetAllAsync(pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {imageFileDirectories.Count()} image directories");

            await _redisCache.StoreRedisCacheData(HttpContext, imageFileDirectories, cancellationToken);
            return Ok(imageFileDirectories);
        }
        [HttpPut]
        [Route(nameof(ImageFileDirectory))]
        [ProducesResponseType(typeof(ImageFileDirectory), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ProductType>> CreateImageFileDirectoryAsync(ImageFileDirectory imageFileDirectory, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateImageFileDirectoryAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            await _imageFileRepository.AddAsync(imageFileDirectory, cancellationToken);
            _logger.LogInformation($"User = {user}. Create image file directory, id : {imageFileDirectory.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, imageFileDirectory, cancellationToken);
            return Ok(imageFileDirectory);
        }
        [HttpDelete]
        [Route(nameof(ImageFileDirectory) + "/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<bool>> RemoveImageFileDirectoryAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RemoveImageFileDirectoryAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var deleted = await _imageFileRepository.DeleteByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Deleted image file directory, id : {id}");

            await _redisCache.RemoveRedisCacheDataAsync(cancellationToken);
            return deleted ? Ok(deleted) : NotFound();
        }
        #endregion

    }
}
