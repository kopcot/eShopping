using eShopping.ServiceClient.Components;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace eShopping.ServiceClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHttpClient();

            builder.Services.AddHealthChecksUI(setupSettings: setup =>
            {
                setup.MaximumHistoryEntriesPerEndpoint(50)
                     .SetMinimumSecondsBetweenFailureNotifications(300)
                     .SetEvaluationTimeInSeconds(30);
            })
                //.AddInMemoryStorage();
                .AddMySqlStorage(builder.Configuration["ConnectionStrings:DefaultConnection"]!,
                    configureOptions: congfigure =>
                    {
                        congfigure
                            .ConfigureWarnings(b => b.Log(
                                (RelationalEventId.ConnectionOpened, LogLevel.Debug),
                                (RelationalEventId.ConnectionClosed, LogLevel.Debug),
                                (RelationalEventId.CommandExecuting, LogLevel.Debug),
                                (RelationalEventId.CommandExecuted, LogLevel.Debug)));
                    });

            builder.Services.AddOpenTelemetry()
                .UseOtlpExporter(OtlpExportProtocol.HttpProtobuf, new(builder.Configuration["ConnectionStrings:OtlpExporter"]!))
                .ConfigureResource(resource => resource.AddService(
                    serviceName: "eShopping.ServiceClient",
                    serviceInstanceId: Environment.MachineName
                    ))
                .WithTracing(tracing =>
                {
                    tracing
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("eShopping.ServiceClient"))
                        .SetSampler(new AlwaysOnSampler())
                        .AddSource("eShopping.ServiceClient")
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
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("eShopping.ServiceClient"))
                        .AddRuntimeInstrumentation()
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation();
                });

            builder.Services.AddSingleton(TracerProvider.Default.GetTracer("eShopping.ServiceClient"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();

            app.UseHealthChecksUI();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
