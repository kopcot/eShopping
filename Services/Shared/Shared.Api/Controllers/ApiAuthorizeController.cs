using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Security;
using System.Net;

namespace Shared.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class ApiAuthorizeController : ControllerBase , IDisposable
    {
        protected readonly ILogger _logger;
        protected readonly IBaseSecurity _baseSecurity;
        private bool disposedValue;

        public ApiAuthorizeController(IBaseSecurity baseSecurity, ILogger logger)
        {
            _baseSecurity = baseSecurity;
            _logger = logger;
        }

        [Route("{**catchAll}")]
        [AllowAnonymous]
        [HttpDelete]
        [HttpGet]
        [HttpHead]
        [HttpOptions]
        [HttpPatch]
        [HttpPost]
        [HttpPut]
        [ProducesResponseType(typeof(Exception), (int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<string>> CatchAllAsync(string catchAll)
        {
            ProblemDetails problemDetails = new()
            {
                Status = StatusCodes.Status404NotFound,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = $"Undefined route",
                Detail = $"{catchAll}",
            };
            return await Task.FromResult(new NotFoundObjectResult(problemDetails));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
        
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        
        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApiAuthorizeController()
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
    }
}
