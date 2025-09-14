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
            
            // Standard HTTP attributes
            activity?.SetTag("http.method", "POST");
            activity?.SetTag("http.scheme", Request.Scheme);
            activity?.SetTag("http.host", Request.Host.ToString());
            activity?.SetTag("http.route", "/TracingEndpoints?handler=RequestA");
            activity?.SetTag("http.user_agent", Request.Headers["User-Agent"].ToString());
            
            // Service and endpoint attributes
            activity?.SetTag("service.name", "dotnet-quickstart");
            activity?.SetTag("service.version", "1.0.0");
            activity?.SetTag("endpoint.name", "RequestA");
            activity?.SetTag("endpoint.type", "entry_point");
            
            // Custom business attributes
            activity?.SetTag("business.operation", "distributed_tracing_demo");
            activity?.SetTag("business.domain", "observability");
            activity?.SetTag("business.priority", "high");
            
            // Performance and monitoring attributes
            var startTime = DateTime.UtcNow;
            activity?.SetTag("performance.expected_duration_ms", 200);
            activity?.SetTag("monitoring.enabled", true);
            activity?.SetTag("debug.trace_id", activity?.TraceId.ToString());
            
            _logger.LogInformation("RequestA started");
            
            // Simulate some work
            await Task.Delay(50);
            
            // Call RequestB directly (not via HTTP to maintain trace context)
            var requestBResult = await CallRequestB();
            
            // Add completion attributes
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("performance.actual_duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("business.result", "success");
            activity?.SetTag("business.processed_items", 1);
            
            _logger.LogInformation("RequestA completed");
            return new JsonResult(new { status = $"RequestA completed, {requestBResult}" });
        }

        public async Task<IActionResult> OnPostRequestB()
        {
            using var activity = Program.MyActivitySource.StartActivity("RequestB", ActivityKind.Internal);
            
            // Service and endpoint attributes
            activity?.SetTag("service.name", "dotnet-quickstart");
            activity?.SetTag("service.version", "1.0.0");
            activity?.SetTag("endpoint.name", "RequestB");
            activity?.SetTag("endpoint.type", "internal_processing");
            
            // Custom business attributes
            activity?.SetTag("business.operation", "data_processing");
            activity?.SetTag("business.domain", "observability");
            activity?.SetTag("business.priority", "medium");
            activity?.SetTag("business.process_type", "intermediate");
            
            // Performance attributes
            var startTime = DateTime.UtcNow;
            activity?.SetTag("performance.expected_duration_ms", 100);
            activity?.SetTag("performance.operation_type", "cpu_intensive");
            
            // Data processing attributes
            activity?.SetTag("data.input_size", "medium");
            activity?.SetTag("data.processing_type", "sequential");
            activity?.SetTag("data.validation_required", true);
            
            _logger.LogInformation("RequestB started");
            
            // Simulate some work
            await Task.Delay(30);
            
            // Call RequestC directly (not via HTTP to maintain trace context)
            var requestCResult = await CallRequestC();
            
            // Add completion attributes
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("performance.actual_duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("business.result", "success");
            activity?.SetTag("data.output_size", "small");
            activity?.SetTag("data.validation_passed", true);
            
            _logger.LogInformation("RequestB completed");
            return new JsonResult(new { status = $"RequestB completed, {requestCResult}" });
        }

        public IActionResult OnPostRequestC()
        {
            using var activity = Program.MyActivitySource.StartActivity("RequestC", ActivityKind.Internal);
            
            // Service and endpoint attributes
            activity?.SetTag("service.name", "dotnet-quickstart");
            activity?.SetTag("service.version", "1.0.0");
            activity?.SetTag("endpoint.name", "RequestC");
            activity?.SetTag("endpoint.type", "final_processing");
            
            // Custom business attributes
            activity?.SetTag("business.operation", "final_validation");
            activity?.SetTag("business.domain", "observability");
            activity?.SetTag("business.priority", "low");
            activity?.SetTag("business.process_type", "terminal");
            
            // Performance attributes
            var startTime = DateTime.UtcNow;
            activity?.SetTag("performance.expected_duration_ms", 50);
            activity?.SetTag("performance.operation_type", "memory_intensive");
            
            // Data processing attributes
            activity?.SetTag("data.input_size", "small");
            activity?.SetTag("data.processing_type", "validation");
            activity?.SetTag("data.validation_required", false);
            activity?.SetTag("data.cleanup_required", true);
            
            // System attributes
            activity?.SetTag("system.component", "validator");
            activity?.SetTag("system.resource_type", "memory");
            activity?.SetTag("system.criticality", "low");
            
            _logger.LogInformation("RequestC started");
            
            // Simulate some work
            System.Threading.Thread.Sleep(20);
            
            // Add completion attributes
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("performance.actual_duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("business.result", "success");
            activity?.SetTag("data.output_size", "minimal");
            activity?.SetTag("data.validation_passed", true);
            activity?.SetTag("data.cleanup_completed", true);
            activity?.SetTag("system.status", "healthy");
            
            _logger.LogInformation("RequestC completed");
            return new JsonResult(new { status = "RequestC completed" });
        }

        // Helper methods for direct calls (maintains trace context)
        private async Task<string> CallRequestB()
        {
            using var activity = Program.MyActivitySource.StartActivity("CallRequestB", ActivityKind.Internal);
            
            // Internal call attributes
            activity?.SetTag("operation.type", "internal_call");
            activity?.SetTag("operation.target", "RequestB");
            activity?.SetTag("operation.purpose", "orchestration");
            
            // Performance attributes
            var startTime = DateTime.UtcNow;
            activity?.SetTag("performance.expected_duration_ms", 50);
            
            // Call context attributes
            activity?.SetTag("call.source", "RequestA");
            activity?.SetTag("call.method", "direct_invocation");
            activity?.SetTag("call.async", true);
            
            // Simulate some work
            await Task.Delay(25);
            
            // Call RequestC
            var requestCResult = await CallRequestC();
            
            // Add completion attributes
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("performance.actual_duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("call.result", "success");
            activity?.SetTag("call.cascading_calls", 1);
            
            return $"RequestB processed, {requestCResult}";
        }

        private async Task<string> CallRequestC()
        {
            using var activity = Program.MyActivitySource.StartActivity("CallRequestC", ActivityKind.Internal);
            
            // Internal call attributes
            activity?.SetTag("operation.type", "internal_call");
            activity?.SetTag("operation.target", "RequestC");
            activity?.SetTag("operation.purpose", "final_processing");
            
            // Performance attributes
            var startTime = DateTime.UtcNow;
            activity?.SetTag("performance.expected_duration_ms", 30);
            
            // Call context attributes
            activity?.SetTag("call.source", "CallRequestB");
            activity?.SetTag("call.method", "direct_invocation");
            activity?.SetTag("call.async", true);
            activity?.SetTag("call.chain_position", "terminal");
            
            // Simulate some work
            await Task.Delay(15);
            
            // Add completion attributes
            var duration = DateTime.UtcNow - startTime;
            activity?.SetTag("performance.actual_duration_ms", duration.TotalMilliseconds);
            activity?.SetTag("call.result", "success");
            activity?.SetTag("call.cascading_calls", 0);
            
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
/*
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
            
        }*/
    }
}