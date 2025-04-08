using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace MyLab.Api.OTLP;

public class OpenTelemetryCommon
{
    // Custom metrics for the application
    public static readonly Meter GreeterMeter = new Meter("OTel.Example", "1.0.0");
    public static readonly Counter<int> CountGreetings = GreeterMeter.CreateCounter<int>("greetings.count", description: "Counts the number of greetings");

    // Custom ActivitySource for the application
    public static readonly ActivitySource GreeterActivitySource = new ActivitySource("OTel.Example");
    public static readonly ActivitySource GreeterActivitySource2 = new ActivitySource("OTel.Example2");
}