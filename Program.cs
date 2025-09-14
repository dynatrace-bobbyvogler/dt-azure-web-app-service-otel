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

        private static void initOpenTelemetry(IServiceCollection services)
        {
            Console.WriteLine("=== OpenTelemetry Initialization Starting ===");
            Console.WriteLine($"ActivitySource: {MyActivitySource.Name}");

            // Simple OpenTelemetry configuration for OneAgent integration
            services.AddOpenTelemetry()
                .WithTracing(builder => {
                    builder
                        .AddSource(MyActivitySource.Name);
                });
            
            Console.WriteLine("=== OpenTelemetry Initialization Complete ===");
            Console.WriteLine("OpenTelemetry configured for OneAgent integration");
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