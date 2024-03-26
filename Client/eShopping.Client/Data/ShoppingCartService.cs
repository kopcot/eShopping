using Basket.Core.Entities;
using System.Net;

namespace eShopping.Client.Data
{
    public class ShoppingCartService : IShoppingCartService, IDisposable
    {
        private readonly HttpResponseResolver _httpResponseResolver;
        private readonly string? _routeAPI;
        private readonly ILogger<ShoppingCartService> _logger;

        private bool disposedValue;

        public ShoppingCartService(string? addressIP, string? routeAPI, HttpClient httpClient, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(addressIP);
            ArgumentNullException.ThrowIfNull(routeAPI);
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            _routeAPI = routeAPI;

            _logger = loggerFactory.CreateLogger<ShoppingCartService>();

            _httpResponseResolver = new HttpResponseResolver(httpClient, addressIP, _logger, httpContextAccessor);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetShoppingCartsAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ShoppingCart
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, long?)> GetShoppingCartsCountAsync<T>(CancellationToken? cancellationToken) where T : ShoppingCart
        {
            return await _httpResponseResolver.ResolveGetResponse<long?>(_routeAPI + typeof(T).Name + "/count", cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken) where T : ShoppingCart
        {
            return await _httpResponseResolver.ResolveGetResponse<T?>(_routeAPI + typeof(T).Name + "/" + shoppingCartId, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdAsync<T>(int shoppingCartId, CancellationToken? cancellationToken) where T : ShoppingCartItem
        {
            return await _httpResponseResolver.ResolveGetResponse<T?>(_routeAPI + typeof(T).Name + "/" + shoppingCartId, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetShoppingCartItemByIdsAsync<T>(int shoppingCartId, int shoppingCartItemId, CancellationToken? cancellationToken) where T : ShoppingCartItem
        {
            return await _httpResponseResolver.ResolveGetResponse<T?>(_routeAPI + typeof(T).Name + "/" + shoppingCartId + "/" + shoppingCartItemId, cancellationToken);
        }
        #region Dispose
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                _httpResponseResolver.Dispose();

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~ShoppingCartService()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion Dispose
    }
}
