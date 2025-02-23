using Basket.Infrastructure.Extensions;
using Shared.Api.Extensions;

namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            if ((Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is string runningInContainer) &&
                runningInContainer.Equals("true"))
                builder.Configuration.AddJsonFile("appsettings.Container.json", optional: true, reloadOnChange: true);
            else
                builder.Configuration.AddJsonFile("appsettings.LocalNetwork.json", optional: true, reloadOnChange: true);

            //builder.Services.AddOutputCache(option => option.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60)); //slowed down results on NAS

            // Add services to the container.
            builder.Services.AddControllers(options =>
            { 
                options.Filters.Add<OperationCancelledExceptionFilter>();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.ToString());
            });
            builder.Services.AddInfrastructureServices(
                connectionType: builder.Configuration["ConnectionStrings:ConnectionType"],
                connectionStringDB: builder.Configuration["ConnectionStrings:DefaultConnection"],
                connectionStringRedis: builder.Configuration["ConnectionStrings:RedisDB"],
                connectionStringOtlpExporter: builder.Configuration["ConnectionStrings:OtlpExporter"],
                timeout: new TimeSpan(0, 1, 0));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseInfrastructureServices();

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseOutputCache(); //slowed down results on NAS
            app.MapControllers();

            app.Run();
        }
    }
}

