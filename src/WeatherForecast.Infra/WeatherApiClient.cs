using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;

namespace WeatherForecast.Infra;

public class WeatherApiClient : IWeatherApi
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherApiClient> _logger;

    public WeatherApiClient(HttpClient httpClient, ILogger<WeatherApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<ForecastModel> GetForecastAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        var requestUri = new Uri($"v1/forecast?latitude={latitude}&longitude={longitude}&hourly=apparent_temperature",
            UriKind.Relative);

        try
        {
            var response = await _httpClient.GetFromJsonAsync<WeatherApiResponse>(requestUri, ct);
            return response.ToForecastModel();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when querying Weather API for {Latitude} {Longitude}", latitude, longitude);
            throw;
        }
    }
}