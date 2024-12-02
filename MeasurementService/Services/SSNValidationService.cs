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
                var combinedPolicy = Policy.WrapAsync(_retryPolicy, _timeoutPolicy);

                var response = await combinedPolicy.ExecuteAsync(() =>
                    _httpClient.GetAsync($"validate-ssn/{ssn}"));

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"SSN validation failed with response: {content}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating SSN: {ex.Message}");
                return false;
            }
        }
    }
}
