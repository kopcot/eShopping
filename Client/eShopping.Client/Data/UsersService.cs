using System.Net;
using Users.Core.Entities;

namespace eShopping.Client.Data
{
    public class UsersService : IUsersService, IDisposable
    {
        private readonly HttpResponseResolver _httpResponseResolver;
        private readonly string? _routeAPI;
        private readonly ILogger<ShoppingCartService> _logger;

        private bool disposedValue;

        public UsersService(string? addressIP, string? routeAPI, HttpClient httpClient, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(addressIP);
            ArgumentNullException.ThrowIfNull(routeAPI);
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            _routeAPI = routeAPI;

            _logger = loggerFactory.CreateLogger<ShoppingCartService>();

            _httpResponseResolver = new HttpResponseResolver(httpClient, addressIP, _logger, httpContextAccessor);
        }
        public async Task<(bool, HttpStatusCode?, Exception?, bool)> GetCheckUserAsync<T>(string username , string password, CancellationToken? cancellationToken) where T : User
        {
            T? user = new User() { Name = username, Password = password  } as T;
            return await _httpResponseResolver.ResolvePostAsJsonResponse<T, bool>(_routeAPI + typeof(T).Name + "/check", user!, cancellationToken);
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
        ~UsersService()
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
