using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

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
            _logger.LogInformation("RequestA started");
            // Simulate some logic for distributed tracing
            // Call RequestB endpoint
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await httpClient.PostAsync($"{baseUrl}/TracingEndpoints?handler=RequestB", null);
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("RequestA completed");
                return new JsonResult(new { status = $"RequestA completed, {result}" });
            }
        }

        public async Task<IActionResult> OnPostRequestB()
        {
            _logger.LogInformation("RequestB started");
            // Simulate some logic for distributed tracing
            // Call RequestC endpoint
            using (var httpClient = new System.Net.Http.HttpClient())
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var response = await httpClient.PostAsync($"{baseUrl}/TracingEndpoints?handler=RequestC", null);
                var result = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("RequestB completed");
                return new JsonResult(new { status = $"RequestB completed, {result}" });
            }
        }

        public IActionResult OnPostRequestC()
        {
            _logger.LogInformation("RequestC started");
            // Simulate some logic for distributed tracing
            _logger.LogInformation("RequestC completed");
            return new JsonResult(new { status = "RequestC completed" });
        }
    }
}
