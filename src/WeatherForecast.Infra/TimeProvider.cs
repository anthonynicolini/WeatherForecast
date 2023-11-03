using Microsoft.Extensions.Internal;

namespace WeatherForecast.Infra;

public class TimeProvider: ISystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}