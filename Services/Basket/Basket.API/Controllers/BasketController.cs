using Basket.Core.Entities;
using Basket.Core.Specs;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using Shared.Api.Controllers;
using Shared.Core.Responses;
using Shared.Core.Specs;
using Shared.Infrastructure.Data;
using System.Net;

namespace Basket.API.Controllers
{
    public class BasketController : ApiAuthorizeController
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IUserService _userService;
        private readonly IRedisCache _redisCache;
        public BasketController(IShoppingCartRepository shoppingCartRepository,
            IShoppingCartItemRepository shoppingCartItemRepository,
            IRedisCache redisCache,
            IUserService userService,
            ILogger<BasketController> logger, 
            Tracer tracer) : base(userService, logger, tracer)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _userService = userService;
            _redisCache = redisCache;
        }
        #region Shopping cart
        [HttpGet]
        [Route(nameof(ShoppingCart))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ShoppingCart>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> GetAllShoppingCartsAsync([FromQuery] Pagination? pagination, [FromQuery] ShoppingCartSpecParams catalogSpecParam, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllShoppingCartsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var shoppingCarts) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ShoppingCart>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(shoppingCarts);

            shoppingCarts = await _shoppingCartRepository.GetFilteredAsync(catalogSpecParam, pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {shoppingCarts.Count()} shopping carts");

            foreach (var shoppingCart in shoppingCarts)
            {
                FillCountAndTotalPrice(shoppingCart);
                shoppingCart.Items = [];
            }
            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCarts, cancellationToken);
            return Ok(shoppingCarts);
        }
        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(ShoppingCart) + "/{id}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ShoppingCart?>> GetShoppingCartByIdAsync([FromRoute]int id, [FromQuery] ShoppingCartItemSpecParams catalogSpecParam, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartByIdAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var shoppingCart) = await _redisCache.GetRedisCacheDataAsync<ShoppingCart>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(shoppingCart);

            _logger.LogInformation($"User = {user}. Return shopping cart with id = {id}");
            shoppingCart = await _shoppingCartRepository.GetByIdAsync(id, catalogSpecParam, cancellationToken);
            _logger.LogInformation($"User = {user}. Return shopping cart exists: {shoppingCart is not null}");

            FillCountAndTotalPrice(shoppingCart);

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCart, cancellationToken);
            return shoppingCart is null ? NotFound() : Ok(shoppingCart);
        }


        [HttpGet]
        [Route(nameof(ShoppingCart) + "/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(ApiResponse<long>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ApiResponse<long>>> GetCountShoppingCartsAsync(CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetCountShoppingCartsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(ApiResponse<long>.CreateApiResponse(count, user));

            count = await _shoppingCartRepository.GetCountAsync(cancellationToken);
            _logger.LogInformation($"User = {user}. Return count of shopping carts");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(ApiResponse<long>.CreateApiResponse(count, user));
        }

        [HttpPut]
        [Route(nameof(ShoppingCart))]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<ShoppingCart>> CreateShoppingCartAsync(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateShoppingCartAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var created = await _shoppingCartRepository.AddAsync(shoppingCart, cancellationToken);
            _logger.LogInformation($"User = {user}. Create shopping cart, id : {shoppingCart.Id}");

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCart, cancellationToken);
            return created ? Created(string.Empty, shoppingCart) : BadRequest();
        }
        [HttpDelete]
        [Route(nameof(ShoppingCart) + "/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<bool>> RemoveShoppingCartTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RemoveShoppingCartTypeAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var deleted = await _shoppingCartRepository.DeleteByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Deleted shopping cart, id : {id}");

            await _redisCache.RemoveRedisCacheDataAsync(cancellationToken);
            return deleted ? Ok(deleted) : NotFound();
        }

        private static void FillCountAndTotalPrice(ShoppingCart? shoppingCart)
        {
            if (shoppingCart != null)
            {
                shoppingCart.TotalPrice = shoppingCart.Items.Sum(sci => sci.IsDeleted ? 0 : sci.Price * sci.Quantity);
                shoppingCart.ItemCount = shoppingCart.Items.Count;
            }
        }
        #endregion

        #region Shopping cart items
        [HttpGet]
        [Route(nameof(ShoppingCartItem))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ShoppingCartItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<ShoppingCartItem>>> GetAllShoppingCartItemsAsync([FromQuery] Pagination? pagination, [FromQuery] ShoppingCartItemSpecParams catalogSpecParam, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllShoppingCartItemsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var shoppingCartItems) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ShoppingCartItem>>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(shoppingCartItems);

            shoppingCartItems = await _shoppingCartItemRepository.GetFilteredAsync(catalogSpecParam, pagination, cancellationToken);
            _logger.LogInformation($"User = {user}. Return {shoppingCartItems.Count()} shopping carts items");

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCartItems, cancellationToken);
            return Ok(shoppingCartItems);
        }
        [HttpGet]
        [Route(nameof(ShoppingCartItem) + "/{id}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(ShoppingCartItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ShoppingCartItem>> GetAllShoppingCartItemByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetAllShoppingCartItemByIdAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var shoppingCartItem) = await _redisCache.GetRedisCacheDataAsync<ShoppingCartItem>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(shoppingCartItem);

            _logger.LogInformation($"User = {user}. Return shopping cart items with id = {id}");
            shoppingCartItem = await _shoppingCartItemRepository.GetByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Return shopping cart items exists: {shoppingCartItem is not null}");

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCartItem, cancellationToken);
            return shoppingCartItem is null ? NotFound() : Ok(shoppingCartItem);
        }
        [HttpGet]
        [Route(nameof(ShoppingCartItem) + "/{shoppingCartId}/{shoppingCartItemId}")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(ShoppingCartItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<long>> GetShoppingCartItemByIdsAsync(int shoppingCartId, int shoppingCartItemId, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartItemByIdsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var shoppingCartItem) = await _redisCache.GetRedisCacheDataAsync<ShoppingCartItem>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(shoppingCartItem);

            _logger.LogInformation($"User = {user}. Return shopping cart item with shoppingCartId = {shoppingCartId} and shoppingCartItemId = {shoppingCartItemId}");
            shoppingCartItem = await _shoppingCartItemRepository.GetByIdsAsync(shoppingCartId, shoppingCartItemId, cancellationToken);
            _logger.LogInformation($"User = {user}. Return shopping cart item exists: {shoppingCartItem is not null}");

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCartItem, cancellationToken);
            return shoppingCartItem is null ? NotFound() : Ok(shoppingCartItem);
        }
        [HttpGet]
        [Route(nameof(ShoppingCartItem) + "/{id}/count")]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(long), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<long>> GetCountShoppingCartItemsAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetCountShoppingCartItemsAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            (var isCached, var count) = await _redisCache.GetRedisCacheDataAsync<long>(HttpContext, cancellationToken);
            if (isCached)
                return Ok(count);

            count = await _shoppingCartItemRepository.GetCountAsync(id);
            _logger.LogInformation($"User = {user}. Return count of shopping carts items");

            await _redisCache.StoreRedisCacheData(HttpContext, count, cancellationToken);
            return Ok(count);
        }
        [HttpDelete]
        [Route(nameof(ShoppingCartItem) + "/{id}")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<bool>> RemoveProductTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            using var span = _tracer.StartActiveSpan(nameof(RemoveProductTypeAsync));

            (var result, var user, var exception) = await _userService.GetUser(Request);
            if (!result)
                return StatusCode((int)HttpStatusCode.InternalServerError, exception);

            var deleted = await _shoppingCartItemRepository.DeleteByIdAsync(id, cancellationToken);
            _logger.LogInformation($"User = {user}. Deleted shopping cart item, id : {id}");

            await _redisCache.RemoveRedisCacheDataAsync(cancellationToken);
            return deleted ? Ok(deleted) : NotFound();
        }
        #endregion
    }
}
