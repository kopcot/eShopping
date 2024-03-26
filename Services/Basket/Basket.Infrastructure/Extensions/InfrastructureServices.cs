using Basket.Infrastructure.Data;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Extensions;

namespace Basket.Infrastructure.Extensions
{
    public static class InfrastructureServices
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection serviceCollection,
            string? connectionType,
            string? connectionStringDB,
            string? connectionStringRedis,
            TimeSpan? timeout = null)
        {
            // NOT GOOD-PRACTICE , should not be used
            // It will include only services which have already been added to the collection
            // must be called in the correct order
            ILoggerFactory logFactory = serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();

            serviceCollection.AddHealthChecks<BasketContext>(timeout);
            serviceCollection.AddDbContextPool<BasketContext>(connectionType, connectionStringDB, "Basket.Infrastructure", logFactory);

            serviceCollection.AddIpRateLimit();
            serviceCollection.AddRedisCache(connectionStringRedis, "Basket.Infrastructure");

            serviceCollection.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            serviceCollection.AddScoped<IShoppingCartItemRepository, ShoppingCartItemRepository>();
            serviceCollection.AddScoped<IUserService, UserService>();

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
