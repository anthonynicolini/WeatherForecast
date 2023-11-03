using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;

namespace WeatherForecast.Core;

public class ForecastService: IForecastService
{
    private readonly IForecastRepository _forecastRepository;
    private readonly IWeatherApi _weatherApi;
    private readonly ISystemClock _clock;
    private readonly ILogger<ForecastService> _logger;

    public ForecastService(IForecastRepository forecastRepository,
        IWeatherApi weatherApi,
        ISystemClock clock,
        ILogger<ForecastService> logger)
    {
        _forecastRepository = forecastRepository;
        _weatherApi = weatherApi;
        _clock = clock;
        _logger = logger;
    }
    
    public async Task<ForecastModel> ProvideForecastAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        var existing = await _forecastRepository.FindAsync(latitude, longitude, ct);
        if (existing is not null && existing.UpdatedOn > _clock.UtcNow.AddDays(-1))
        {
            return existing;
        }
        
        _logger.LogInformation("Database data unavailable, attempting to retrieve from Weather API");
        var result = await _weatherApi.GetForecastAsync(latitude, longitude, ct);
        result.UpdatedOn = _clock.UtcNow;
        await _forecastRepository.AddOrUpdateAsync(result, ct);
        
        return result;
    }
    
    public Task<IEnumerable<ForecastModel>> GetAllForecastsAsync(CancellationToken ct)
    { 
        return _forecastRepository.GetAllAsync(ct);
    }

    public Task<ForecastModel?> GetExistingForecastAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        return _forecastRepository.FindAsync(latitude, longitude, ct);
    }

    public Task<ForecastModel> CreateForecastAsync(ForecastModel create, CancellationToken ct)
    {
        create.UpdatedOn = _clock.UtcNow;
        return _forecastRepository.AddOrUpdateAsync(create, ct);
    }

    public Task<ForecastModel> UpdateForecastAsync(ForecastModel update, CancellationToken ct)
    {
        update.UpdatedOn = _clock.UtcNow;
        return _forecastRepository.AddOrUpdateAsync(update, ct);
    }

    public Task DeleteForecastAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        return _forecastRepository.DeleteAsync(latitude, longitude, ct);
    }
}