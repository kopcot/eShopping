using Catalog.Core.Entities;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using OpenTelemetry.Trace;
using Shared.Core.Responses;
using System.Net;

namespace eShopping.Client.Data
{
    public class CatalogService : BaseHttpClientService<CatalogService>, ICatalogService
    {

        public CatalogService(string? addressIP, string? routeAPI, HttpClient httpClient, ILogger<CatalogService> logger, IHttpContextAccessor httpContextAccessor, Tracer tracer)
            : base(addressIP, routeAPI, httpClient, logger, httpContextAccessor, tracer)
        {
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductsAsync<T>(CancellationToken? cancellationToken) where T : Product
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductsAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductByQueryAsync<T>(string queryString, CancellationToken? cancellationToken) where T : Product
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductByQueryAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, long?)> GetProductCountAsync<T>(string queryString, CancellationToken? cancellationToken) where T : Product
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductCountAsync));
            return await _httpResponseResolver.ResolveGetResponse<long?>(_routeAPI + typeof(T).Name + "/Count" + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> GetProductByIdAsync<T>(int productId, CancellationToken? cancellationToken) where T : Product
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductByIdAsync));
            return await _httpResponseResolver.ResolveGetResponse<T?>(_routeAPI + typeof(T).Name + "/" + productId, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductAsync<T1, T2>(T1 product, CancellationToken? cancellationToken) where T1 : Product
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductAsync));
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, product, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductBrandAsync<T1, T2>(T1 productBrand, CancellationToken? cancellationToken = null) where T1 : ProductBrand
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductAsync));
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, productBrand, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductTypeAsync<T1, T2>(T1 productType, CancellationToken? cancellationToken = null) where T1 : ProductType
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductAsync));
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, productType, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductBrandsAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ProductBrand
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductBrandsAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductTypesAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ProductType
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductTypesAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductImageFoldersAsync<T>(string queryString, CancellationToken? cancellationToken) where T : ImageFileDirectory
        {
            using var span = _tracer.StartActiveSpan(nameof(GetProductImageFoldersAsync));
            return await _httpResponseResolver.ResolveGetResponse<IEnumerable<T>?>(_routeAPI + typeof(T).Name + queryString, cancellationToken);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductImageFoldersAsync<T1, T2>(T1 imageFileDirectory, CancellationToken? cancellationToken) where T1 : ImageFileDirectory
        {
            using var span = _tracer.StartActiveSpan(nameof(CreateProductImageFoldersAsync));
            return await _httpResponseResolver.ResolvePutAsJsonResponse<T1, T2?>(_routeAPI + typeof(T1).Name, imageFileDirectory, cancellationToken);
        }
    }
}
