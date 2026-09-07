using FaultMonitoring.Worker.Consumers;
using FaultMonitoring.Worker.Extensions;
using MassTransit;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog((services, configuration) =>
    configuration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

builder.Services.AddOpenTelemetryTracing();

builder.Services.AddMassTransit(registration =>
{
    registration.AddConsumer<ServiceFaultDetectedIntegrationEventConsumer>();

    registration.UsingRabbitMq((context, bus) =>
    {
        var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var port = ushort.TryParse(
            builder.Configuration["RabbitMQ:Port"],
            out var configuredPort)
                ? configuredPort
                : (ushort)5672;
        var username = builder.Configuration["RabbitMQ:Username"]
            ?? throw new InvalidOperationException(
                "RabbitMQ:Username configuration is required.");
        var password = builder.Configuration["RabbitMQ:Password"]
            ?? throw new InvalidOperationException(
                "RabbitMQ:Password configuration is required.");

        bus.Host(
            host,
            port,
            builder.Configuration["RabbitMQ:VirtualHost"] ?? "/",
            rabbit =>
            {
                rabbit.Username(username);
                rabbit.Password(password);
            });

        bus.ReceiveEndpoint(
            "minicommerce-fault-monitor",
            endpoint =>
            {
                endpoint.PrefetchCount = 32;
                endpoint.ConcurrentMessageLimit = 8;
                endpoint.ConfigureConsumer<
                    ServiceFaultDetectedIntegrationEventConsumer>(context);
            });
    });
});

var host = builder.Build();
await host.RunAsync();
