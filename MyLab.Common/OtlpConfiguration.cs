using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.Api.OTLP;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MyLab.Common;

public class OtlpConfiguration
{
     public static void AddOTLP(WebApplicationBuilder builder)
    {
        var otlpEndpoint = builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"];
        AddOtelLogs(builder, otlpEndpoint);
        AddOtelMetrics(builder, otlpEndpoint);
        AddOtelTracing(builder, otlpEndpoint);
    }

    private static void AddOtelTracing(WebApplicationBuilder builder, string? otlpEndpoint)
    {
        // Add Tracing for ASP.NET Core and our custom ActivitySource and export via OTLP
        builder.Services.AddOpenTelemetry().WithTracing(tracing =>
        {
            tracing.AddAspNetCoreInstrumentation(opts => 
            {
                opts.RecordException = true;
                opts.EnrichWithException = (activity, exception) =>
                {
                    activity.SetTag("error.type", exception.GetType().Name);
                    activity.SetTag("error.message", exception.Message);
                };
            });            
            tracing.AddHttpClientInstrumentation();
            // SQL
            tracing.AddSource(OpenTelemetryCommon.GreeterActivitySource.Name);
            tracing.AddSource(OpenTelemetryCommon.GreeterActivitySource2.Name);
            if (builder.Environment.IsDevelopment())
            {
                tracing.AddConsoleExporter();
            }
            if (otlpEndpoint != null)
            {
                tracing.AddOtlpExporter();
            }
        });
    }

    private static void AddOtelMetrics(WebApplicationBuilder builder, string? otlpEndpoint)
    {
        // Add Metrics for ASP.NET Core and our custom metrics and export via OTLP
        builder.Services.AddOpenTelemetry().WithMetrics(metrics =>
        {
            metrics.AddAspNetCoreInstrumentation();
            metrics.AddMeter(OpenTelemetryCommon.GreeterMeter.Name);
            metrics.AddMeter("Microsoft.AspNetCore.Hosting");
            metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
            if (otlpEndpoint != null)
            {
                metrics.AddOtlpExporter();
            }
        });
    }

    private static void AddOtelLogs(WebApplicationBuilder builder, string? otlpEndpoint)
    {
        // Setup logging to be exported via OpenTelemetry
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;

            if (otlpEndpoint != null)
            {
                logging.AddOtlpExporter(); // OTEL_LOG_LEVEL=None
            }
        });
    }
}

