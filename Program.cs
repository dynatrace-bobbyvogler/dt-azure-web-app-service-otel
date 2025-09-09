using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;

namespace MyFirstAzureWebApp
{
    public class Program
    {
        // Dynatrace/OpenTelemetry configuration
        private static string DT_API_URL = ""; // TODO: Provide your SaaS/Managed URL here
        private static string DT_API_TOKEN = ""; // TODO: Provide the OpenTelemetry-scoped access token here

        private const string activitySource = "Dynatrace.DotNetApp.Sample"; // TODO: Provide a descriptive name for your application here
        public static readonly ActivitySource MyActivitySource = new ActivitySource(activitySource);
        private static ILoggerFactory loggerFactoryOT;

        private static void initOpenTelemetry(IServiceCollection services)
        {
            List<KeyValuePair<string, object>> dt_metadata = new List<KeyValuePair<string, object>>();
            foreach (string name in new string[] {"dt_metadata_e617c525669e072eebe3d0f08212e8f2.properties",
                                                "/var/lib/dynatrace/enrichment/dt_metadata.properties",
                                                "/var/lib/dynatrace/enrichment/dt_host_metadata.properties"}) {
                try {
                    foreach (string line in System.IO.File.ReadAllLines(name.StartsWith("/var") ? name : System.IO.File.ReadAllText(name))) { 
                        var keyvalue = line.Split("=");
                        dt_metadata.Add( new KeyValuePair<string, object>(keyvalue[0], keyvalue[1]));
                    }
                }
                catch { }
            }
            
            Action<ResourceBuilder> configureResource = r => r
                .AddService(serviceName: "dotnet-quickstart") //TODO Replace with the name of your application
                .AddAttributes(dt_metadata);
            
            services.AddOpenTelemetry()
                .ConfigureResource(configureResource)
                .WithTracing(builder => {
                    builder
                        .SetSampler(new AlwaysOnSampler())
                        .AddSource(MyActivitySource.Name)
                        .AddOtlpExporter(options => 
                        {
                            options.Endpoint = new Uri(Environment.GetEnvironmentVariable("DT_API_URL")+ "/v1/traces");
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                            options.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
                        });
                })
                .WithMetrics(builder => {
                    builder
                        .AddMeter("my-meter")
                        .AddOtlpExporter((OtlpExporterOptions exporterOptions, MetricReaderOptions readerOptions) =>
                        {
                            exporterOptions.Endpoint = new Uri(Environment.GetEnvironmentVariable("DT_API_URL")+ "/v1/metrics");
                            exporterOptions.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
                            exporterOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                            readerOptions.TemporalityPreference = MetricReaderTemporalityPreference.Delta;
                        });
                    });
            
            var resourceBuilder = ResourceBuilder.CreateDefault();
            configureResource!(resourceBuilder);
            
            loggerFactoryOT = LoggerFactory.Create(builder => {
                builder
                    .AddOpenTelemetry(options => {
                        options.SetResourceBuilder(resourceBuilder).AddOtlpExporter(options => {
                            options.Endpoint = new Uri(Environment.GetEnvironmentVariable("DT_API_URL")+ "/v1/logs");
                            options.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
                            options.ExportProcessorType = OpenTelemetry.ExportProcessorType.Batch;
                            options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        });
                    })
                    .AddConsole();
            });
            Sdk.CreateTracerProviderBuilder()
                .SetSampler(new AlwaysOnSampler())
                .AddSource(MyActivitySource.Name)
                .ConfigureResource(configureResource);
            // add-logging
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
