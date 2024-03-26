using System.Net;

namespace eShopping.Client.Data
{
    public interface IHttpResponseResolver
    {
        Task<(bool, HttpStatusCode?, Exception?, T?)> ResolveGetResponse<T>(string requestUri); 
        Task<(bool, HttpStatusCode?, Exception?, T?)> ResolvePutAsJsonResponse<T>(string requestUri, T input);
    }
}
