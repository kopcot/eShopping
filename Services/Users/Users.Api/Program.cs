using Shared.Api.Extensions;
using Users.Infrastructure.Extensions;

namespace Users.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddOutputCache(option => option.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(60)); //slowed down results on NAS

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<OperationCancelledExceptionFilter>();
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddInfrastructureServices(
                connectionType: builder.Configuration["ConnectionStrings:ConnectionType"],
                connectionStringDB: builder.Configuration["ConnectionStrings:DefaultConnection"],
                connectionStringRedis: builder.Configuration["ConnectionStrings:RedisDB"],
                timeout: new TimeSpan(0, 1, 0));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseInfrastructureServices();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseOutputCache(); //slowed down results on NAS
            app.MapControllers();

            app.Run();
        }
    }
}
