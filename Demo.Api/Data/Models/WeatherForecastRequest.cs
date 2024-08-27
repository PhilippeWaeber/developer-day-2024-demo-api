namespace Demo.Api.Data.Models;

public class WeatherForecastRequest
{
    public int Id { get; set; }

    public required WeatherForecast WeatherForecast { get; set; }

    public required string RequestedBy { get; set; }
    public required DateTime RequestedOn { get; set; }
}
