using Basket.Core.Entities;
using eShopping.Client.Pages;
using OpenTelemetry.Trace;
using System.Net;

namespace eShopping.Client.Data
{
    public class ShoppingCartService : BaseHttpClientService<ShoppingCartService>, IShoppingCartService
    {
        public ShoppingCartService(string? addressIP, string? routeAPI, HttpClient httpClient, ILogger<ShoppingCartService> logger, IHttpContextAccessor httpContextAccessor, Tracer tracer) 
            : base(addressIP, routeAPI, httpClient, logger, httpContextAccessor, tracer)
        {
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetShoppingCartsAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ShoppingCart
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartsAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>>($"{_routeAPI}{typeof(T).Name}{queryString}", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, long?)> GetShoppingCartsCountAsync<T>(CancellationToken? cancellationToken) where T : ShoppingCart
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartsCountAsync));
            return await _httpResponseResolver.ResolveGetResponseV2<long?>($"{_routeAPI}{typeof(T).Name}/count", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartByIdAsync<T>(int shoppingCartId, string queryString, CancellationToken? cancellationToken) where T : ShoppingCart
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartByIdAsync));
            return await _httpResponseResolver.ResolveGetResponse<T?>($"{_routeAPI}{typeof(T).Name}/{shoppingCartId}{queryString}", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken) where T : ShoppingCartItem
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartItemByIdAsync));
            return await _httpResponseResolver.ResolveGetResponse<T?>($"{_routeAPI}{typeof(T).Name}/{shoppingCartId}", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdsAsync<T>(int shoppingCartId, int shoppingCartItemId, CancellationToken? cancellationToken) where T : ShoppingCartItem
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartItemByIdsAsync));
            return await _httpResponseResolver.ResolveGetResponse<T?>($"{_routeAPI}{typeof(T).Name}/{shoppingCartId}/{shoppingCartItemId}" , cancellationToken);
        }

        public async Task<(bool, HttpStatusCode?, Exception?, bool)> DeleteShoppingCartByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken = null) where T : ShoppingCart
        {
            using var span = _tracer.StartActiveSpan(nameof(DeleteShoppingCartByIdAsync));
            return await _httpResponseResolver.ResolveDeleteResponse<bool>($"{_routeAPI}{typeof(T).Name}/{shoppingCartId}", cancellationToken);
        }

        public async Task<(bool, HttpStatusCode?, Exception?, bool)> DeleteShoppingCartItemByIdAsync<T>(int shoppingCartItemId, CancellationToken? cancellationToken = null) where T : ShoppingCartItem
        {
            using var span = _tracer.StartActiveSpan(nameof(DeleteShoppingCartItemByIdAsync));
            return await _httpResponseResolver.ResolveDeleteResponse<bool>($"{_routeAPI}{typeof(T).Name}/{shoppingCartItemId}", cancellationToken);
        }

        public async Task<(bool, HttpStatusCode?, Exception?, long?)> GetShoppingCartItemsCountAsync<T>(int shoppingCartId, CancellationToken? cancellationToken = null) where T : ShoppingCartItem
        {
            using var span = _tracer.StartActiveSpan(nameof(GetShoppingCartItemsCountAsync));
            return await _httpResponseResolver.ResolveGetResponse<long?>($"{_routeAPI}{typeof(T).Name}/{shoppingCartId}/count", cancellationToken);
        }
    }
}
