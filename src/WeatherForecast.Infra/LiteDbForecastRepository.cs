using LiteDB;
using WeatherForecast.Core.Interfaces;
using WeatherForecast.Core.Models;

namespace WeatherForecast.Infra;

public class LiteDbForecastRepository : IForecastRepository
{
    private readonly ILiteCollection<ForecastModel> _forecasts;

    public LiteDbForecastRepository(ILiteDatabase db)
    {
        _forecasts = db.GetCollection<ForecastModel>();
    }
    public Task<IEnumerable<ForecastModel>> GetAllAsync(CancellationToken ct)
    {
        return Task.FromResult<IEnumerable<ForecastModel>>(_forecasts.Query().ToList());
    }

    public Task<ForecastModel?> FindAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        var result = _forecasts.FindOne(x => x.Latitude == latitude && x.Longitude == longitude);
        return Task.FromResult(result);
    }

    public Task<ForecastModel> AddOrUpdateAsync(ForecastModel add, CancellationToken ct)
    {
        var result = _forecasts.FindOne(x => x.Latitude == add.Latitude && 
                                             x.Longitude == add.Longitude);

        if (result is not null)
        {
            _forecasts.DeleteMany(x => x.Latitude == add.Latitude && x.Longitude == add.Longitude);
        }

        _forecasts.Insert(add);
        return Task.FromResult(add);
    }

    public Task DeleteAsync(decimal latitude, decimal longitude, CancellationToken ct)
    {
        _forecasts.DeleteMany(x => x.Latitude == latitude && x.Longitude == longitude);
        return Task.CompletedTask;
    }
}