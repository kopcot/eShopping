using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Extensions;
using Users.Infrastructure.Data;
using Users.Infrastructure.Repositories;
using Users.Infrastructure.Security;

namespace Users.Infrastructure.Extensions
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection serviceCollection, 
            string? connectionType,
            string? connectionStringDB,
            string? connectionStringRedis,
            string? connectionStringOtlpExporter,
            TimeSpan? timeout = null)
        {
            // NOT GOOD-PRACTICE , should not be used
            // It will include only services which have already been added to the collection
            // must be called in the correct order
            ILoggerFactory logFactory = serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

            serviceCollection.AddHealthChecks<UserContext>(timeout);
            serviceCollection.AddDbContextPool<UserContext>(connectionType, connectionStringDB, "Users.Infrastructure", logFactory);

            serviceCollection.AddIpRateLimit();
            serviceCollection.AddRedisCache(connectionStringRedis, "Users.Infrastructure");

            serviceCollection.AddOpenTelemetryService("Users.Infrastructure", new(connectionStringOtlpExporter));

            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IIpConnectionRepository, IpConnectionRepository>();
            serviceCollection.AddSingleton<IUserService, UserService>();

            serviceCollection.AddCookieAuthentication();


            return serviceCollection;
        }

        public static void UseInfrastructureServices(this WebApplication app)
        {
            app.UseHealthChecks();
            app.UseIpRateLimit();
        }
    }
}
