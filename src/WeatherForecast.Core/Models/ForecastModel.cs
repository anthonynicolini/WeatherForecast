using System.ComponentModel.DataAnnotations;

namespace WeatherForecast.Core.Models;

public class ForecastModel
{
    private decimal _latitude;
    private decimal _longitude;

    [Required]
    [Range(-90, 90)]
    public decimal Latitude
    {
        get => Math.Round(_latitude, 2); //being more precise would be pointless
        set => _latitude = Math.Round(value, 2);
    }

    [Required]
    [Range(-180, 180)]
    public decimal Longitude
    {
        get => Math.Round(_longitude, 2);
        set => _longitude = Math.Round(value, 2);
    }

    [Required]
    [MaxLength(20)]
    public string Timezone { get; set; }

    public DateTimeOffset UpdatedOn { get; set; }
    
    public IReadOnlyList<DateTime> HourlyTime { get; set; }
    public IReadOnlyList<decimal> HourlyTemperature { get; set; }
}