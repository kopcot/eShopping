using Catalog.Core.Entities;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace eShopping.Client.Data
{
    public interface ICatalogService
    {
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductsAsync<T>(CancellationToken? cancellationToken = null) where T : Product;
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductByQueryAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : Product;
        Task<(bool, HttpStatusCode?, Exception?, long?)> GetProductCountAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : Product;
        Task<(bool, HttpStatusCode?, Exception?, T?)> GetProductByIdAsync<T>(int productId, CancellationToken? cancellationToken = null) where T : Product;
        Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductAsync<T1, T2>(T1 product, CancellationToken? cancellationToken = null) where T1 : Product;
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductBrandsAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : ProductBrand;
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductTypesAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : ProductType;
        Task<(bool, HttpStatusCode?, Exception?, IEnumerable<T>?)> GetProductImageFoldersAsync<T>(string queryString, CancellationToken? cancellationToken = null) where T : ImageFileDirectory;
        Task<(bool, HttpStatusCode?, Exception?, T2?)> CreateProductImageFoldersAsync<T1, T2>(T1 imageFileDirectory, CancellationToken? cancellationToken = null) where T1 : ImageFileDirectory;
    }
}
