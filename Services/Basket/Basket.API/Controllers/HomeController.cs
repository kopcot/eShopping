using Basket.Core.Entities;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Shared.Api.Controllers;
using Shared.Core.Specs;
using Shared.Infrastructure.Data;
using System.Net;

namespace Basket.API.Controllers
{
    public class HomeController : ApiAuthorizeController
    {
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IUserService _userService;
        private readonly IRedisCache _redisCache;
        public HomeController(IShoppingCartRepository shoppingCartRepository,
            IShoppingCartItemRepository shoppingCartItemRepository,
            IRedisCache redisCache,
            IUserService userService,
            ILogger<HomeController> logger) : base(userService, logger)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _userService = userService;
            _redisCache = redisCache;
        }
        [HttpGet]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<ActionResult<bool>> GetAcceptAsync()
        {
            return await Task.FromResult(Ok(true));
        }
        [HttpGet]
        [Route(nameof(ShoppingCart))]
        //[OutputCache] //slowed down results on NAS
        [ProducesResponseType(typeof(IEnumerable<ShoppingCart>), (int)HttpStatusCode.OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> GetAllShoppingCartsAsync([FromQuery] Pagination? pagination)
        {
            (var isCached, var shoppingCarts) = await _redisCache.GetRedisCacheDataAsync<IEnumerable<ShoppingCart>>(HttpContext);
            if (isCached)
            {
                _logger.LogInformation("Get from cachce.");
                return Ok(shoppingCarts);
            }

            var user = "TEST_USER";

            shoppingCarts = await _shoppingCartRepository.GetAllAsync(pagination);
            _logger.LogInformation($"User = {user}. Return {shoppingCarts.Count()} shopping carts");

            await _redisCache.StoreRedisCacheData(HttpContext, shoppingCarts);
            return Ok(shoppingCarts);
        }
    }
}
