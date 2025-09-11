using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MyFirstAzureWebApp.Pages
{
    [IgnoreAntiforgeryToken]
    public class TracingEndpointsModel : PageModel
    {
        private readonly ILogger<TracingEndpointsModel> _logger;

        public TracingEndpointsModel(ILogger<TracingEndpointsModel> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> OnPostRequestA()
        {
            using var activity = Program.MyActivitySource.StartActivity("RequestA", ActivityKind.Server);
            activity?.SetTag("http.method", "POST");
            activity?.SetTag("net.protocol.version", "1.1");
            activity?.SetTag("endpoint.name", "RequestA");
            activity?.SetTag("service.name", "dotnet-quickstart");
            
            _logger.LogInformation("RequestA started");
            
            // Simulate some work
            await Task.Delay(50);
            
            // Call RequestB directly (not via HTTP to maintain trace context)
            var requestBResult = await CallRequestB();
            
            _logger.LogInformation("RequestA completed");
            return new JsonResult(new { status = $"RequestA completed, {requestBResult}" });
        }

        public async Task<IActionResult> OnPostRequestB()
        {
            using var activity = Program.MyActivitySource.StartActivity("RequestB", ActivityKind.Internal);
            activity?.SetTag("http.method", "POST");
            activity?.SetTag("net.protocol.version", "1.1");
            activity?.SetTag("endpoint.name", "RequestB");
            activity?.SetTag("service.name", "dotnet-quickstart");
            
            _logger.LogInformation("RequestB started");
            
            // Simulate some work
            await Task.Delay(30);
            
            // Call RequestC directly (not via HTTP to maintain trace context)
            var requestCResult = await CallRequestC();
            
            _logger.LogInformation("RequestB completed");
            return new JsonResult(new { status = $"RequestB completed, {requestCResult}" });
        }

        public IActionResult OnPostRequestC()
        {
            using var activity = Program.MyActivitySource.StartActivity("RequestC", ActivityKind.Internal);
            activity?.SetTag("http.method", "POST");
            activity?.SetTag("net.protocol.version", "1.1");
            activity?.SetTag("endpoint.name", "RequestC");
            activity?.SetTag("service.name", "dotnet-quickstart");
            
            _logger.LogInformation("RequestC started");
            
            // Simulate some work
            System.Threading.Thread.Sleep(20);
            
            _logger.LogInformation("RequestC completed");
            return new JsonResult(new { status = "RequestC completed" });
        }

        // Helper methods for direct calls (maintains trace context)
        private async Task<string> CallRequestB()
        {
            using var activity = Program.MyActivitySource.StartActivity("CallRequestB", ActivityKind.Internal);
            activity?.SetTag("operation", "internal_call");
            activity?.SetTag("target", "RequestB");
            
            // Simulate some work
            await Task.Delay(25);
            
            // Call RequestC
            var requestCResult = await CallRequestC();
            
            return $"RequestB processed, {requestCResult}";
        }

        private async Task<string> CallRequestC()
        {
            using var activity = Program.MyActivitySource.StartActivity("CallRequestC", ActivityKind.Internal);
            activity?.SetTag("operation", "internal_call");
            activity?.SetTag("target", "RequestC");
            
            // Simulate some work
            await Task.Delay(15);
            
            return "RequestC processed";
        }

        public IActionResult OnGetTestTrace()
        {
            Console.WriteLine("=== Test Trace Endpoint Called ===");
            
            using var activity = Program.MyActivitySource.StartActivity("Test Trace Activity", ActivityKind.Internal);
            activity?.SetTag("test.type", "manual");
            activity?.SetTag("test.purpose", "verification");
            activity?.SetTag("http.method", "GET");
            activity?.SetTag("endpoint.name", "TestTrace");
            
            _logger.LogInformation("Test trace activity created");
            
            // Add some delay to make the trace more visible
            System.Threading.Thread.Sleep(100);
            
            activity?.SetStatus(ActivityStatusCode.Ok);
            _logger.LogInformation("Test trace activity completed");
            
            return new JsonResult(new { 
                status = "Test trace sent to Dynatrace", 
                activityId = activity?.Id,
                traceId = activity?.TraceId,
                spanId = activity?.SpanId,
                timestamp = DateTime.UtcNow
            });
        }

        public async Task<IActionResult> OnGetTestConnection()
        {
            Console.WriteLine("=== Manual Connection Test ===");
            
            var apiUrl = Environment.GetEnvironmentVariable("DT_API_URL");
            var apiToken = Environment.GetEnvironmentVariable("DT_API_TOKEN");
            
            if (string.IsNullOrEmpty(apiUrl) || string.IsNullOrEmpty(apiToken))
            {
                return new JsonResult(new { 
                    success = false, 
                    error = "Environment variables not set" 
                });
            }
            
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Api-Token {apiToken}");
                httpClient.Timeout = TimeSpan.FromSeconds(10);
                
                var tracesEndpoint = $"{apiUrl}/v1/traces";
                Console.WriteLine($"Testing: {tracesEndpoint}");
                
                var response = await httpClient.PostAsync(tracesEndpoint, new StringContent(""));
                var content = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Response: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"Content: {content}");
                
                return new JsonResult(new { 
                    success = response.IsSuccessStatusCode,
                    statusCode = response.StatusCode.ToString(),
                    reasonPhrase = response.ReasonPhrase,
                    content = content,
                    endpoint = tracesEndpoint
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new JsonResult(new { 
                    success = false, 
                    error = ex.Message 
                });
            }
        }
    }
}