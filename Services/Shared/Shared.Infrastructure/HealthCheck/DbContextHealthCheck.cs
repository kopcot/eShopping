using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.Infrastructure.HealthCheck
{
    public class DbContextHealthCheck<T> : IHealthCheck where T : DbContext
    {
        private readonly T _dbContext;

        public DbContextHealthCheck(T dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Perform a test query or operation on your DbContext
                // For example, you can execute a simple query like this:
                await _dbContext.Database.CanConnectAsync(cancellationToken);

                // DbContext is healthy
                return HealthCheckResult.Healthy("DbContext connection test sucessful.");
            }
            catch (Exception ex)
            {
                // If an exception is thrown, return an unhealthy result with the error details
                return HealthCheckResult.Unhealthy("DbContext connection test failed", ex);
            }
        }
    }
}
