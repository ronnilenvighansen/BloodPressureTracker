using Polly;

namespace MeasurementService.Services;

public class PolicyHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;
    private readonly IAsyncPolicy<HttpResponseMessage> _timeoutPolicy;

    public PolicyHandler(IAsyncPolicy<HttpResponseMessage> retryPolicy, IAsyncPolicy<HttpResponseMessage> timeoutPolicy)
    {
        _retryPolicy = retryPolicy;
        _timeoutPolicy = timeoutPolicy;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var combinedPolicy = Policy.WrapAsync(_retryPolicy, _timeoutPolicy);
        return await combinedPolicy.ExecuteAsync(() => base.SendAsync(request, cancellationToken));
    }
}

