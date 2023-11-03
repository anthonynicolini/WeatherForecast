namespace WeatherForecast.Core.Models;

public class WeatherApiResponse
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string Timezone { get; set; }
    public HourlyWeather Hourly { get; set; }

    public class HourlyWeather
    {
        public DateTime[] Time { get; set; }
        public decimal[] Apparent_Temperature { get; set; }
    }

    public ForecastModel ToForecastModel()
    {
        return new ForecastModel
        {
            Latitude = Math.Round(Latitude, 2),
            Longitude = Math.Round(Longitude, 2),
            Timezone = Timezone,
            HourlyTemperature = Hourly.Apparent_Temperature,
            HourlyTime = Hourly.Time
        };
    }
}