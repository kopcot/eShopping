using Catalog.Core.Entities;
using System.Net;

namespace eShopping.Client.Data
{
    public class CatalogService : ICatalogService, IDisposable
    {

        private readonly HttpResponseResolver _httpResponseResolver;
        private readonly string? _routeAPI;
        private readonly ILogger<CatalogService> _logger;

        private bool disposedValue;

        public CatalogService(string? addressIP, string? routeAPI, HttpClient httpClient, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(addressIP);
            ArgumentNullException.ThrowIfNull(routeAPI);
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            _routeAPI = routeAPI;

            _logger = loggerFactory.CreateLogger<CatalogService>();

            _httpResponseResolver = new HttpResponseResolver(httpClient, addressIP, _logger, httpContextAccessor);

        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductsAsync<T>(CancellationToken? cancellationToken) where T : Product
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductByQueryAsync<T>(string queryString, CancellationToken? cancellationToken) where T : Product
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, long?)> GetProductCountAsync<T>(string queryString, CancellationToken? cancellationToken) where T : Product
        {
            return await _httpResponseResolver.ResolveGetResponse<long?>(_routeAPI + typeof(T).Name + "/Count" + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetProductByIdAsync<T>(int productId, CancellationToken? cancellationToken) where T : Product
        {
            return await _httpResponseResolver.ResolveGetResponse<T?>(_routeAPI + typeof(T).Name + "/" + productId, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductAsync<T1, T2>(T1 product, CancellationToken? cancellationToken) where T1 : Product
        {
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, product, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductBrandsAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ProductBrand
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductTypesAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ProductType
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductImageFoldersAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ImageFileDirectory
        {
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductImageFoldersAsync<T1, T2>(T1 imageFileDirectory, CancellationToken? cancellationToken) where T1 : ImageFileDirectory
        {
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, imageFileDirectory, cancellationToken);
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
        ~CatalogService()
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
