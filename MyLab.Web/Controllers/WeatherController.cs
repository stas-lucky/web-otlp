using Microsoft.AspNetCore.Mvc;
using MyLab.Web.OTLP;

namespace MyLab.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController
{
    private readonly ILogger<WeatherController> _logger;

    public WeatherController(ILogger<WeatherController> logger)
    {
        _logger = logger;
    }
    
    string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    
    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Length)]
                })
            .ToArray();

        await SendGreeting();
        return forecast;
    }
    
    async Task<string> SendGreeting()
    {
        // Create a new Activity scoped to the method
        using var activity = Metrics.GreeterActivitySource.StartActivity("GreeterActivity");
        
        _logger.LogInformation("Starting greeting");
        
        Metrics.CountGreetings.Add(1);
        activity?.SetTag("greeting", "Hello World!");
        
        _logger.LogInformation("Greeting sent successfully");


        return "Hello World!";
    }
}