using WeatherForecast.Core.Models;

namespace WeatherForecast.Core.Interfaces;

public interface IForecastRepository
{
    Task<IEnumerable<ForecastModel>> GetAllAsync(CancellationToken ct);
    Task<ForecastModel?> FindAsync(decimal latitude, decimal longitude, CancellationToken ct);
    Task<ForecastModel> AddOrUpdateAsync(ForecastModel add, CancellationToken ct);
    Task DeleteAsync(decimal latitude, decimal longitude, CancellationToken ct);
}