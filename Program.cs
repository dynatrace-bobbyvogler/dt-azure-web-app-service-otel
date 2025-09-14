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
using Microsoft.Extensions.Configuration;


namespace MyFirstAzureWebApp
{
    public class Program
    {
        // Dynatrace /OpenTelemetry configuration
        private const string activitySource = "Dynatrace.DotNetApp.Sample"; // TODO: Provide a descriptive name for your application here
        public static readonly ActivitySource MyActivitySource = new ActivitySource(activitySource);
        private static ILoggerFactory loggerFactoryOT;

        private static void initOpenTelemetry(IServiceCollection services)
        {
            Console.WriteLine("=== OpenTelemetry Initialization Starting ===");
            //Console.WriteLine($"DT_API_URL: {Environment.GetEnvironmentVariable("DT_API_URL")}");
            //Console.WriteLine($"DT_API_TOKEN: {(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DT_API_TOKEN")) ? "NOT SET" : "SET")}");
            Console.WriteLine($"ActivitySource: {MyActivitySource.Name}");

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
                            //var endpoint = Environment.GetEnvironmentVariable("DT_API_URL") + "/v1/traces";
                            //Console.WriteLine($"Tracing Endpoint: {endpoint}");
                            //options.Endpoint = new Uri(endpoint);
                            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                           // options.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
                        });
                })
                .WithMetrics(builder => {
                    builder
                        .AddMeter("my-meter")
                        .AddOtlpExporter((OtlpExporterOptions exporterOptions, MetricReaderOptions readerOptions) =>
                        {
                            //exporterOptions.Endpoint = new Uri(Environment.GetEnvironmentVariable("DT_API_URL")+ "/v1/metrics");
                            //exporterOptions.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
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
                            //options.Endpoint = new Uri(Environment.GetEnvironmentVariable("DT_API_URL")+ "/v1/logs");
                           // options.Headers = $"Authorization=Api-Token {Environment.GetEnvironmentVariable("DT_API_TOKEN")}";
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
            
            Console.WriteLine("=== OpenTelemetry Initialization Complete ===");
            Console.WriteLine("Tracing, Metrics, and Logging configured for Dynatrace");
        }

        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Validate environment variables
                //ValidateEnvironmentVariables();

                // Initialize OpenTelemetry
                initOpenTelemetry(builder.Services);

                // Add services to the container.
                builder.Services.AddRazorPages();

                var app = builder.Build();

                // Validate environment variables
    

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
            catch (Exception ex)
            {
                // This will help identify startup issues
                Console.WriteLine($"Application failed to start: {ex}");
                throw;
            }
            }
    }
}