using LiteDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Internal;
using WeatherForecast.Core;
using WeatherForecast.Core.Interfaces;

namespace WeatherForecast.Infra.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSingleton<ISystemClock, TimeProvider>();
        
        services.AddScoped<IForecastService, ForecastService>();
        services.AddScoped<IForecastRepository, LiteDbForecastRepository>();
        services.AddScoped<IWeatherApi, WeatherApiClient>();

        services.AddSingleton<ILiteDatabase>(new LiteDatabase(config.GetConnectionString("ForecastDb")));
       
        return services;
    }
}