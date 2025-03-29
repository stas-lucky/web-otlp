using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MyLab.Web.OTLP;

public class Metrics
{
    // Custom metrics for the application
    public static readonly Meter GreeterMeter = new Meter("OTel.Example", "1.0.0");
    public static readonly Counter<int> CountGreetings = GreeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

    // Custom ActivitySource for the application
    public static readonly ActivitySource GreeterActivitySource = new ActivitySource("OTel.Example");
}