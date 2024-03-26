using Microsoft.AspNetCore.Authentication.Cookies;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace eShopping.Client.Data
{
    public class HttpResponseResolver : IDisposable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly JsonSerializerOptions _jsonSerializerOptions; 

        private bool disposedValue;
        public HttpResponseResolver(HttpClient httpClient, string addressIP, ILogger logger, IHttpContextAccessor httpContextAccessor)
        {

            httpClient.BaseAddress = new Uri(addressIP);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("API-KEY", "ac3as3c0as8689!cas68b6nz1u6lu#");
            httpClient.DefaultRequestHeaders.Add("userRole", "0");

            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor; 
            
            _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> ResolveGetResponse<T>(string requestUri, CancellationToken? cancellationToken)
        {
            try
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, (CancellationToken)cancellationToken))
                {
                    return await GetResult<T>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> ResolvePutAsJsonResponse<T1, T2>(string requestUri, T1 input, CancellationToken? cancellationToken)
        {
            try
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.PutAsJsonAsync(requestUri, input, _jsonSerializerOptions, (CancellationToken)cancellationToken))
                {
                    return await GetResult<T2>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T2?)> ResolvePostAsJsonResponse<T1, T2>(string requestUri, T1 input, CancellationToken? cancellationToken)
        {
            try
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.PostAsJsonAsync(requestUri, input, _jsonSerializerOptions, (CancellationToken)cancellationToken))
                {
                    return await GetResult<T2>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> ResolveDeleteResponse<T>(string requestUri, CancellationToken? cancellationToken)
        {
            try
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.DeleteAsync(requestUri, (CancellationToken)cancellationToken))
                {
                    return await GetResult<T>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }

        private async Task<(bool, HttpStatusCode?, Exception?, T?)> GetResult<T>(HttpResponseMessage response, CancellationToken? cancellationToken)
        {
            if (response.IsSuccessStatusCode)
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                // TEST WITH JsonSerializer
                T? result = default;
                await using (var content = await response.Content.ReadAsStreamAsync((CancellationToken)cancellationToken))
                {
                    result = await JsonSerializer.DeserializeAsync<T>(content, _jsonSerializerOptions, (CancellationToken)cancellationToken);
                }
                //var result = await response.Content.ReadFromJsonAsync<T>();
                return (true, response.StatusCode, null, result);
            }
            else
            {
                string msg = await response.Content.ReadAsStringAsync();
                _logger.LogError(msg);
                return (false, response.StatusCode, null, default);
            }
        }

        private async Task SetResponseHeader()
        {
            var claims = _httpContextAccessor.HttpContext!.User.Claims;
            var username = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            var role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            var headerValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:test:{role}"));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(CookieAuthenticationDefaults.AuthenticationScheme, headerValue);

            await Task.CompletedTask;
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

                _httpClient.Dispose();

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~HttpResponseResolver()
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
