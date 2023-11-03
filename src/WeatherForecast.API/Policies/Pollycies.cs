using Polly;
using Polly.Extensions.Http;

namespace WeatherForecast.API.Policies;

public static class Pollycies
{
    public static IAsyncPolicy<HttpResponseMessage> ExponentialRetry()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}