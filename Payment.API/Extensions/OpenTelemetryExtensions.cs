using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Payment.API.Extensions;

public static class OpenTelemetryExtensions
{
    public static IServiceCollection AddOpenTelemetryTracing(
        this IServiceCollection services)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                  .AddService(
                      serviceName: "Payment.API",
                      serviceVersion: "1.0.0");
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    .AddSource("MassTransit")
                    .AddOtlpExporter();
            });

        return services;
    }
}