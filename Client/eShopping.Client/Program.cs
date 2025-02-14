using eShopping.Client.Data;
using eShopping.Client.Data.HealthCheck;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace eShopping.Client
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

            var app = BuildServices(builder);

            RunApp(app);
        }

        private static WebApplication BuildServices(WebApplicationBuilder builder)
        {
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<IErrorService, ErrorService>();


            // IHttpContextAccessor for Claim identities
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddHttpClient<ICatalogService, CatalogService>(
                (httpClient, service) => new CatalogService(
                        builder.Configuration["ConnectionData:CatalogApi:Server"],
                        builder.Configuration["ConnectionData:CatalogApi:RouteAPI"],
                        httpClient,
                        service.GetRequiredService<ILogger<CatalogService>>(),
                        service.GetRequiredService<IHttpContextAccessor>(),
                        service.GetRequiredService<Tracer>()))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            builder.Services.AddHttpClient<IShoppingCartService, ShoppingCartService>(
                (httpClient, service) => new ShoppingCartService(
                        builder.Configuration["ConnectionData:BasketApi:Server"],
                        builder.Configuration["ConnectionData:BasketApi:RouteAPI"],
                        httpClient,
                        service.GetRequiredService<ILogger<ShoppingCartService>>(),
                        service.GetRequiredService<IHttpContextAccessor>(),
                        service.GetRequiredService<Tracer>()))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            builder.Services.AddHttpClient<IUsersService, UsersService>(
                (httpClient, service) => new UsersService(
                        builder.Configuration["ConnectionData:UsersApi:Server"],
                        builder.Configuration["ConnectionData:UsersApi:RouteAPI"],
                        httpClient,
                        service.GetRequiredService<ILogger<UsersService>>(),
                        service.GetRequiredService<IHttpContextAccessor>(),
                        service.GetRequiredService<Tracer>()))
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));

            builder.Services.AddHealthChecks()
                .AddTypeActivatedCheck<GCInfoHealthCheck>(
                    name: nameof(GCInfoHealthCheck),
                    failureStatus: HealthStatus.Unhealthy,
                    args: new object[] { (double)512, (double)256 },
                    tags: new[] { "Unhealthy = 512MB", "Degraded = 256MB" },
                    timeout: new TimeSpan(0, 1, 0));

            builder.Services.AddAuthentication(options => options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(configureOptions: options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.AccessDeniedPath = "/forbidden";
                })
                .AddIdentityCookies(
                    options => options.ApplicationCookie = new Microsoft.Extensions.Options.OptionsBuilder<CookieAuthenticationOptions>(
                        builder.Services, "test")
                    );
            builder.Services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
                options.OnAppendCookie = cookieContext =>
                {
                    Console.WriteLine("Cookie appended: " + cookieContext.CookieName);
                };
                options.OnDeleteCookie = context =>
                {
                    Console.WriteLine("Cookie deleted: " + context.CookieName);
                };
            });
            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddOpenTelemetry()
                .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new(builder.Configuration["ConnectionStrings:OtlpExporter"]!))
                .ConfigureResource(resource => resource.AddService(
                    serviceName: "eShopping.Client",
                    serviceInstanceId: Environment.MachineName
                    ))
                .WithTracing(tracing =>
                {
                    tracing
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("eShopping.Client"))
                        .SetSampler(new AlwaysOnSampler())
                        .AddSource("eShopping.Client")
                        // Metrics provides by ASP.NET Core in .NET 8
                        .AddSource("Microsoft.AspNetCore")
                        .AddSource("Microsoft.AspNetCore.Hosting")
                        .AddSource("Microsoft.AspNetCore.Server.Kestrel")
                        //.AddSource("System.Net.Http.HttpClient.health-checks")
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        .AddRedisInstrumentation()
                        .AddConnectorNet();
                    builder.Services.Configure<AspNetCoreTraceInstrumentationOptions>(options =>
                        {
                            options.RecordException = true;
                        });
                })
                .WithMetrics(metrics =>
                {
                    metrics
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("eShopping.Client"))
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.Services.AddSingleton(TracerProvider.Default.GetTracer("eShopping.Client"));

            return builder.Build();
        }


        private static void RunApp(WebApplication app)
        {
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.MapHealthChecks("/api/health");
            app.MapHealthChecks("/api/health/detail", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
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

            app.UseStaticFiles();
            // added for content folder

            if (Environment.GetEnvironmentVariable("PHYSICALFOLDER_LOCATION") is string PHYSICALFOLDER_LOCATION)
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    //FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Content")),
                    FileProvider = new PhysicalFileProvider(PHYSICALFOLDER_LOCATION),
                    RequestPath = new PathString("")
                });
            }

            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");


            app.Run();
        }
    }
}
