// In Services/SSNValidationService.cs
using Polly;

namespace MeasurementService.Services
{
    public class SSNValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
        private readonly IAsyncPolicy<HttpResponseMessage> _timeoutPolicy;

        public SSNValidationService(HttpClient httpClient, 
            IAsyncPolicy<HttpResponseMessage> retryPolicy, 
            IAsyncPolicy<HttpResponseMessage> timeoutPolicy)
        {
            _httpClient = httpClient;
            _retryPolicy = retryPolicy;
            _timeoutPolicy = timeoutPolicy;
        }

        public async Task<bool> ValidateSSNAsync(string ssn)
        {
            try
            {
                // Combine retry and timeout policies
                var combinedPolicy = Policy.WrapAsync(_retryPolicy, _timeoutPolicy);

                // Apply resilience policies
                var response = await combinedPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"validate-ssn/{ssn}"));

                // Log response details for debugging
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // Log non-successful response for debugging
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SSN validation failed with response: {content}");
                return false;
            }
            catch (Exception ex)
            {
                // Log and handle failure
                Console.WriteLine($"Error validating SSN: {ex.Message}");
                return false; // Treat as invalid if service is unavailable
            }
        }
    }
}
