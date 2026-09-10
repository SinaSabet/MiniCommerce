using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Api.Gateway.Extensions
{
    public static class OpenTelemetryExtensions
    {

        public static IServiceCollection AddGatewayObservability(
        this IServiceCollection services)
        {

            services
                .AddOpenTelemetry()
                .ConfigureResource(resource =>
                {
                    resource
                        .AddService(
                            serviceName: "MiniCommerce.Gateway.API",
                            serviceVersion: "1.0.0");
                })


                .WithTracing(tracing =>
                {
                    tracing

                        // Incoming HTTP requests
                        .AddAspNetCoreInstrumentation()


                        // Gateway -> Microservices
                        .AddHttpClientInstrumentation()


                        // MassTransit propagation
                        .AddSource("MassTransit")


                        // Send traces to collector
                        .AddOtlpExporter();

                })


                .WithMetrics(metrics =>
                {

                    metrics

                        .AddAspNetCoreInstrumentation()

                        .AddHttpClientInstrumentation()

                        .AddRuntimeInstrumentation()

                        .AddProcessInstrumentation()

                        .AddMeter("MassTransit")

                        .AddOtlpExporter();

                });


            return services;

        }

    }
}
