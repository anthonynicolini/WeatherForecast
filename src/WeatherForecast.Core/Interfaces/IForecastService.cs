using WeatherForecast.Core.Models;

namespace WeatherForecast.Core.Interfaces;

public interface IForecastService
{
    Task<ForecastModel> ProvideForecastAsync(decimal latitude, decimal longitude, CancellationToken ct);
    Task<IEnumerable<ForecastModel>> GetAllForecastsAsync(CancellationToken ct);
    Task<ForecastModel?> GetExistingForecastAsync(decimal latitude, decimal longitude, CancellationToken ct);
    Task<ForecastModel> CreateForecastAsync(ForecastModel create, CancellationToken ct);
    Task<ForecastModel> UpdateForecastAsync(ForecastModel update, CancellationToken ct);
    Task DeleteForecastAsync(decimal latitude, decimal longitude, CancellationToken ct);
}