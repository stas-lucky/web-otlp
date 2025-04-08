using System.ComponentModel.DataAnnotations;
using Refit;

namespace MyLab.Client.Web.Clients;

public interface IMyLabApiClient
{
    [Get("/weather")]
    Task<IEnumerable<WeatherForecastResponse>> GetWeather();
    
    
    [Get("/weather/ex")]
    Task<IEnumerable<WeatherForecastResponse>> GetWeatherWithException();
}

public class WeatherForecastResponse
{
    public DateOnly Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF { get; set; }

    public string? Summary { get; set; }
}