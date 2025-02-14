using OpenTelemetry.Trace;
using System.Diagnostics;

namespace eShopping.Client.Data
{
    public abstract class BaseHttpClientService<T> : IDisposable, IBaseService 
    {
        protected readonly HttpResponseResolver _httpResponseResolver;
        protected readonly string? _routeAPI;
        protected readonly string? _addressIP;
        protected readonly ILogger<T> _logger;
        protected readonly Tracer _tracer;
        private readonly TelemetrySpan _telemetrySpan;

        protected BaseHttpClientService(string? addressIP, string? routeAPI, HttpClient httpClient, ILogger<T> logger, IHttpContextAccessor httpContextAccessor, Tracer tracer)
        {
            ArgumentNullException.ThrowIfNull(addressIP);
            ArgumentNullException.ThrowIfNull(routeAPI);
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(tracer);

            _routeAPI = routeAPI;
            _addressIP = addressIP;

            _logger = logger;

            _httpResponseResolver = new HttpResponseResolver(httpClient, addressIP, _logger, httpContextAccessor, tracer);

            _tracer = tracer;
            _telemetrySpan = _tracer.StartActiveSpan($"{nameof(BaseHttpClientService<T>)}_{typeof(T).Name}");
        }
        public string? AddressIP => _addressIP;
        public string? RouteAPI => _routeAPI;

        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                _telemetrySpan.Dispose();
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~BaseService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
