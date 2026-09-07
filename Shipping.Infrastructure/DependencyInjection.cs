using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shipping.Application.Interfaces;
using Shipping.Application.Services;
using Shipping.Domain.Shipments;
using Shipping.Infrastructure.Messaging.Consumers;
using Shipping.Infrastructure.Messaging.Observers;
using Shipping.Infrastructure.Persistence;
using Shipping.Infrastructure.Persistence.Repositories;

namespace Shipping.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ShippingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ShippingConnection")));

        services.AddScoped<IShipmentRepository, ShipmentRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddReceiveObserver<ServiceFaultReceiveObserver>();

        services.AddMassTransit(registration =>
        {
            registration.AddConsumer<
                ShippingRequestedIntegrationEventConsumer>();

            registration.AddEntityFrameworkOutbox<ShippingDbContext>(outbox =>
            {
                outbox.UseSqlServer();
                outbox.UseBusOutbox();
            });

            registration.UsingRabbitMq((context, bus) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var port = ushort.TryParse(
                    configuration["RabbitMQ:Port"],
                    out var configuredPort)
                        ? configuredPort
                        : (ushort)5672;

                bus.Host(
                    host,
                    port,
                    configuration["RabbitMQ:VirtualHost"] ?? "/",
                    rabbit =>
                    {
                        rabbit.Username(
                            configuration["RabbitMQ:Username"] ?? "guest");
                        rabbit.Password(
                            configuration["RabbitMQ:Password"] ?? "guest");
                    });

                bus.UseMessageRetry(retry =>
                    retry.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromSeconds(1),
                        maxInterval: TimeSpan.FromSeconds(30),
                        intervalDelta: TimeSpan.FromSeconds(5)));

                bus.ReceiveEndpoint(
                    "shipping-requested",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            ShippingRequestedIntegrationEventConsumer>(
                            context);
                    });
            });
        });

        return services;
    }
}
