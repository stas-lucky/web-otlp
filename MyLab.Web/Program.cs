using MyLab.Web.OTLP;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MyLab.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        AddOTLP(builder);
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();
        app.MapControllers();


        app.Run();
    }

    private static void AddOTLP(WebApplicationBuilder builder)
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
            tracing.AddSource(Metrics.GreeterActivitySource.Name);
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
            metrics.AddMeter(Metrics.GreeterMeter.Name);
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
                logging.AddOtlpExporter();
            }
        });
    }
}