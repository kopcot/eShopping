using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Shared.Infrastructure.Data
{
    public class PerformanceInterceptor : DbCommandInterceptor
    {
        private readonly ILogger _logger;
        private readonly long _querySlowThreshold; // milliseconds
        public PerformanceInterceptor(ILoggerFactory logger, long querySlowThresholdMs = 200)
        {
            _logger = logger.CreateLogger<PerformanceInterceptor>();
            _querySlowThreshold = querySlowThresholdMs;
        }
        public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData eventData, DbDataReader result)
        {
            var originalResult = base.ReaderExecuted(command, eventData, result);

            if (eventData.Duration.TotalMilliseconds > _querySlowThreshold)
            {
                _logger.LogWarning($"Slow ReaderExecuted Detected: {command.CommandText}");
                _logger.LogWarning($"EventData.Duration.TotalMilliseconds: {eventData.Duration.TotalMilliseconds}");
            }

            return originalResult;
        }
        public override async ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData eventData, DbDataReader result, CancellationToken cancellationToken = default)
        {
            var originalResult = await base.ReaderExecutedAsync(command, eventData, result, cancellationToken);

            if (eventData.Duration.TotalMilliseconds > _querySlowThreshold)
            {
                _logger.LogWarning($"Slow ReaderExecutedAsync Detected: {command.CommandText}");
                _logger.LogWarning($"EventData.Duration.TotalMilliseconds: {eventData.Duration.TotalMilliseconds}");
            }

            return originalResult;
        }
    }
}
