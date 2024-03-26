using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Diagnostics;

namespace Shared.Infrastructure.HealthCheck
{
    public class GCInfoHealthCheck : IHealthCheck
    {
        private readonly double? _thresholdInMegaBytes = null;
        private readonly double? _degradedInMegaBytes = null;
        public GCInfoHealthCheck(double? thresholdInMegaBytes = null, double? degradedInMegaBytes = null)
        {
            _thresholdInMegaBytes = thresholdInMegaBytes;
            _degradedInMegaBytes = degradedInMegaBytes;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            const double fromBtoMB = 1024 * 1024;
            double allocated = (double)GC.GetTotalMemory(forceFullCollection: false) / fromBtoMB;
            double totalCommittedBytes = (double)GC.GetGCMemoryInfo().TotalCommittedBytes / fromBtoMB;
            double totalAvailableMemoryBytes = (double)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / fromBtoMB;
            double privateMemorySize64 = 0.0;
            double workingSet64 = 0.0;
            int pid = 0;
            using (var process = Process.GetCurrentProcess())
            {
                privateMemorySize64 = (double)process.PrivateMemorySize64 / fromBtoMB;
                pid = process.Id;
                workingSet64 = (double)process.WorkingSet64 / fromBtoMB;
            }
            var data = new Dictionary<string, object>()
                {
                    { "Gen0Collections", GC.CollectionCount(0) },
                    { "Gen1Collections", GC.CollectionCount(1) },
                    { "Gen2Collections", GC.CollectionCount(2) },
                    { "Allocated_inMegaBytes", allocated },
                    { "TotalCommittedBytes_inMegaBytes", totalCommittedBytes },
                    { "TotalAvailableMemoryBytes_inMegaBytes", totalAvailableMemoryBytes },
                    { "PrivateMemorySize64_inMegaBytes", privateMemorySize64 },
                    { "WorkingSet64_inMegaBytes", workingSet64 },
                    { "Process_ID", pid },
                };

            HealthStatus result;
            if (allocated > _thresholdInMegaBytes)
            { 
                result = HealthStatus.Unhealthy;
                // NOT GOOD-PRACTICE , should not be used
                // used for docker with limited usage of RAM
                GC.Collect();
            }
            else if (allocated > _degradedInMegaBytes)
                result = HealthStatus.Degraded;
            else
                result = HealthStatus.Healthy;

            await Task.CompletedTask;

            return new HealthCheckResult(
                result,
                description: "Actual allocated memory by GC = " + allocated.ToString("0.00") + " MB (for compare); Woking set = " + workingSet64.ToString("0.00") + " MB",
                data: data);

        }
    }
}
