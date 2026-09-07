using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace FaultMonitoring.Worker.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("FaultMonitoring.Worker"))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddSource("MassTransit")
                .AddOtlpExporter())
            .WithMetrics(metrics => metrics
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddMeter("MassTransit")
                .AddOtlpExporter());

        return services;
    }
}
