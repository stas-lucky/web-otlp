using Microsoft.AspNetCore.Mvc;
using MyLab.Client.Web.Clients;

namespace MyLab.Client.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController(IMyLabApiClient apiClient)
{
    [HttpGet]
    public async Task<IEnumerable<WeatherForecastResponse>> Get()
    {
        var res  = await apiClient.GetWeather();
        return res;
    }
    
    
    [HttpGet("ex")]
    public async Task<IEnumerable<WeatherForecastResponse>> GetWithException()
    {
        var res  = await apiClient.GetWeatherWithException();
        return res;
    }
}