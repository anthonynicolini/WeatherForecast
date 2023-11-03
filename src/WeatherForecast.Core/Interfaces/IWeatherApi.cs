using WeatherForecast.Core.Models;

namespace WeatherForecast.Core.Interfaces;

public interface IWeatherApi
{
    Task<ForecastModel> GetForecastAsync(decimal latitude, decimal longitude, CancellationToken ct);
}