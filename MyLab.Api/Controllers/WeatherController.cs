using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyLab.Api.OTLP;
using MyLab.Common;

namespace MyLab.Api.Controllers;

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
    
    [HttpGet("ex")]
    public async Task<string> GetWithException()
    {
        await SendGreeting(true);
        return "ERROR!";
    }
    
    async Task<string> SendGreeting(bool throwException = false)
    {
        // Create a new Activity scoped to the method
        using var activity = OpenTelemetryCommon.GreeterActivitySource.StartActivity("GreeterActivity");
        using var activity2 = OpenTelemetryCommon.GreeterActivitySource2.StartActivity("GreeterActivity2");

        _logger.LogInformation("Starting greeting");
        
        OpenTelemetryCommon.CountGreetings.Add(1);
        Activity.Current?.SetTag("greeting", "Hello World!");
        if (throwException)
        {
            var ex = new Exception("Oops! Inside the span!");
            Activity.Current?.AddException(ex, new TagList()
            {
                {"tag1key", "tag1value"}
            });
            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            throw ex;
        }
        
        _logger.LogInformation("Greeting sent successfully");


        return "Hello World!";
    }
}