using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Repositories;
using Catalog.Infrastructure.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Infrastructure.Extensions;

namespace Catalog.Infrastructure.Extensions
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
            
            serviceCollection.AddHealthChecks<CatalogContext>(timeout);
            serviceCollection.AddDbContextPool<CatalogContext>(connectionType, connectionStringDB, "Catalog.Infrastructure", logFactory);

            serviceCollection.AddIpRateLimit();
            serviceCollection.AddRedisCache(connectionStringRedis, "Catalog.Infrastructure");

            serviceCollection.AddScoped<IProductRepository, ProductRepository>();
            serviceCollection.AddScoped<IProductTypeRepository, ProductTypeRepository>();
            serviceCollection.AddScoped<IProductBrandRepository, ProductBrandRepository>();
            serviceCollection.AddScoped<IImageFileRepository, ImageFileRepository>();
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
