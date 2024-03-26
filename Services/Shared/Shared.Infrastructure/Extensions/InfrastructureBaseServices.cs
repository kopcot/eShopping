using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Shared.Infrastructure.HealthCheck;
using Shared.Infrastructure.Security;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shared.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using AspNetCoreRateLimit;
using System.Net;
using Google.Protobuf.WellKnownTypes;
using System.Reflection;
using Microsoft.Extensions.Caching.Distributed;

namespace Shared.Infrastructure.Extensions
{
    public static class InfrastructureBaseServices
    {
        public static void AddHealthChecks<T>(this IServiceCollection serviceCollection, TimeSpan? timeout) where T : DbContext
        {
            serviceCollection.AddHealthChecks()
                .AddAsyncCheck(
                    name: nameof(InfrastructureBaseServices),
                    check: async (cancellationToken) =>
                    {
                        return await Task.FromResult(HealthCheckResult.Healthy("A healthy result."));
                    },
                    timeout: timeout)
                .AddCheck<DbContextHealthCheck<T>>(
                    name: nameof(DbContextHealthCheck<T>),
                    tags: [ "DbContext" ],
                    timeout: timeout)
                .AddTypeActivatedCheck<GCInfoHealthCheck>(
                    name: nameof(GCInfoHealthCheck),
                    failureStatus: HealthStatus.Unhealthy,
                    args: [(double)256, (double)128],
                    tags: [ "Unhealthy = 256MB", "Degraded = 128MB" ],
                    timeout: timeout ?? new TimeSpan(0, 1, 0))
                .AddDbContextCheck<T>(
                    name: "DbContext",
                    tags: [ "DbContext" ],
                    customTestQuery: async (db, cancel) =>
                    {
                        return await db.Database.CanConnectAsync(cancel);
                    });
        }
        public static void AddDbContextPool<T>(this IServiceCollection serviceCollection, 
            string? connectionType,
            string? connectionString,
            string? migrationAssembly,
            ILoggerFactory logFactory) where T : DbContext
        {
            ArgumentNullException.ThrowIfNull(connectionType);
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(migrationAssembly);

            serviceCollection.AddDbContextPool<T>(options =>
            //serviceCollection.AddDbContext<T>(options =>
            {
                switch (connectionType)
                {
                    case "MSSQL":
                        options.UseSqlServer(
                            connectionString,
                            sqlOptionsBuilder => sqlOptionsBuilder.MigrationsAssembly(migrationAssembly));
                        break;
                    case "MySQL":
                        options.UseMySQL(
                            connectionString,
                            sqlOptionsBuilder => sqlOptionsBuilder.MigrationsAssembly(migrationAssembly));
                        break;
                    default:
                        throw new ArgumentException(connectionType, nameof(connectionType));
                }
                options
                    .AddInterceptors([new PerformanceInterceptor(logFactory)])
                    .ConfigureWarnings(b => b.Log(
                        (RelationalEventId.ConnectionOpened, LogLevel.Debug),
                        (RelationalEventId.ConnectionClosed, LogLevel.Debug),
                        (RelationalEventId.CommandExecuting, LogLevel.Debug),
                        (RelationalEventId.CommandExecuted, LogLevel.Debug)));
            },
            poolSize: 32);
            //contextLifetime: ServiceLifetime.Scoped,
            //optionsLifetime: ServiceLifetime.Scoped);
        }
        public static void AddCookieAuthentication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(CookieAuthenticationDefaults.AuthenticationScheme, null);
        }

        public static void AddIpRateLimit(this IServiceCollection serviceCollection)
        {
            // needed to store rate limit counters and ip rules
            serviceCollection.AddMemoryCache();

            // general configuration
            serviceCollection.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = (int)HttpStatusCode.TooManyRequests;
                options.RealIpHeader = "X-Real-IP";
                options.ClientIdHeader = "X-ClientId";
                options.GeneralRules =
                [
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        //Period = "10s",
                        PeriodTimespan = TimeSpan.FromSeconds(10),
                        Limit = 25,
                        MonitorMode = false
                    },
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        //Period = "1s",
                        PeriodTimespan = TimeSpan.FromSeconds(1),
                        Limit = 5,
                        MonitorMode = true,
                    },
                ];

            });

            // configuration (resolvers, counter key builders)
            serviceCollection.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            // inject counter and rules stores
            serviceCollection.AddInMemoryRateLimiting();
        }

        public static void UseHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/api/health");
            app.MapHealthChecks("/api/health/detail", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) => {
                    HealthReport reportNew = new(
                        entries: report.Entries,
                        // workaround for receiving data, instead of 
                        // status get by the last entry state
                        status: (HealthStatus)Math.Min((int)report.Entries.Min(e => e.Value.Status), (int)report.Status),
                        totalDuration: report.TotalDuration
                        );
                    // test with System.Text.Json.JsonSerializer
                    //var result = await System.Net.Http.Json.JsonContent.Create(reportNew, typeof(HealthReport)).ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Serialize(reportNew);

                    context.Response.Headers.ContentType = "application/json";
                    await context.Response.WriteAsync(result);
                },
                ResultStatusCodes =
                {
                    // workaround for receiving data, instead of 
                    // HTTP response is not in valid state (ServiceUnavailable) when trying to get report from <URL:path> configured with name <Name>.
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status200OK
                }
            });
        }

        public static void UseIpRateLimit(this WebApplication app)
        {
            app.UseIpRateLimiting();
        }
        public static void AddRedisCache(this IServiceCollection serviceCollection, string? redisConnectionString, string instanceName)
        {
            ArgumentNullException.ThrowIfNull(redisConnectionString);

            serviceCollection.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = redisConnectionString;
                option.InstanceName = instanceName;
            });
            serviceCollection.AddScoped<IRedisCache, RedisCache>(option => 
                new (option.GetRequiredService<IDistributedCache>(), 
                     TimeSpan.FromMinutes(5)));
        }
    }
}
