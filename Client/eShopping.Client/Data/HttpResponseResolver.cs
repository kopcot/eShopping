using Microsoft.AspNetCore.Authentication.Cookies;
using OpenTelemetry.Trace;
using Shared.Core.Responses;
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
        private readonly Tracer _tracer;
        private readonly TelemetrySpan _telemetrySpan;

        private bool disposedValue;
        public HttpResponseResolver(HttpClient httpClient, string addressIP, ILogger logger, IHttpContextAccessor httpContextAccessor, Tracer tracer)
        {

            httpClient.BaseAddress = new Uri(addressIP);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("API-KEY", "ac3as3c0as8689!cas68b6nz1u6lu#");
            httpClient.DefaultRequestHeaders.Add("userRole", "0");

            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _tracer = tracer;

            _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

            _telemetrySpan = _tracer.StartActiveSpan(nameof(HttpResponseResolver));
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> ResolveGetResponse<T>(string requestUri, CancellationToken? cancellationToken)
        {
            try
            {
                using var span = _tracer.StartActiveSpan(nameof(ResolveGetResponse));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("GetAsync");
                    return await GetResult<T>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> ResolveGetResponseV2<T>(string requestUri, CancellationToken? cancellationToken)
        {
            try
            {
                using var span = _tracer.StartActiveSpan(nameof(ResolveGetResponseV2));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("GetAsync");
                    return await GetResultV2<T>(response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return (false, null, ex, default);
            }
        }
        public async Task<(bool, HttpStatusCode?, Exception?, T?)> ResolvePutResponse<T>(string requestUri, CancellationToken? cancellationToken)
        {
            try
            {
                using var span = _tracer.StartActiveSpan(nameof(ResolvePutResponse));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.PutAsync(requestUri, null, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("PutAsync");
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
                using var span = _tracer.StartActiveSpan(nameof(ResolvePutAsJsonResponse));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.PutAsJsonAsync(requestUri, input, _jsonSerializerOptions, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("PutAsJsonAsync");
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
                using var span = _tracer.StartActiveSpan(nameof(ResolvePostAsJsonResponse));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.PostAsJsonAsync(requestUri, input, _jsonSerializerOptions, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("PostAsJsonAsync");
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
                using var span = _tracer.StartActiveSpan(nameof(ResolveDeleteResponse));
                cancellationToken ??= new CancellationTokenSource().Token;
                await SetResponseHeader();
                using (var response = await _httpClient.DeleteAsync(requestUri, (CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("DeleteAsync");
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
            using var span = _tracer.StartActiveSpan(nameof(GetResult));
            if (response.IsSuccessStatusCode)
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                // TEST WITH JsonSerializer
                T? result = default;
                await using (var content = await response.Content.ReadAsStreamAsync((CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("ReadAsStreamAsync");
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
        private async Task<(bool, HttpStatusCode?, Exception?, T?)> GetResultV2<T>(HttpResponseMessage response, CancellationToken? cancellationToken)
        {
            using var span = _tracer.StartActiveSpan(nameof(GetResult));
            if (response.IsSuccessStatusCode)
            {
                cancellationToken ??= new CancellationTokenSource().Token;
                // TEST WITH JsonSerializer
                ApiResponse<T?>? result = default;
                await using (var content = await response.Content.ReadAsStreamAsync((CancellationToken)cancellationToken))
                {
                    using var spanPart = _tracer.StartActiveSpan("ReadAsStreamAsync");
                    result = await JsonSerializer.DeserializeAsync<ApiResponse<T?>>(content, _jsonSerializerOptions, (CancellationToken)cancellationToken);
                }
                //var result = await response.Content.ReadFromJsonAsync<T>();
                return (true, response.StatusCode, null, result != null ? result.ResultValue : default);
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
            using var span = _tracer.StartActiveSpan(nameof(SetResponseHeader));
            var claims = _httpContextAccessor?.HttpContext?.User.Claims ?? Enumerable.Empty<Claim>();
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


                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                _httpClient.Dispose();
                _telemetrySpan.Dispose();
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
