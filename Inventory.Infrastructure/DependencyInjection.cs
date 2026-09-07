using Inventory.Application.Interfaces;
using Inventory.Application.Services;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using Inventory.Infrastructure.Messaging.Consumers;
using Inventory.Infrastructure.Messaging.Observers;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Persistence.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<InventoryDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString(
                    "InventoryConnection"));
        });


        services.AddScoped<
            IInventoryItemRepository,
            InventoryItemRepository>();

        services.AddScoped<
            IInventoryReservationRepository,
            InventoryReservationRepository>();

        services.AddScoped<
            IUnitOfWork,
            UnitOfWork>();


        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddReceiveObserver<ServiceFaultReceiveObserver>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ReserveInventoryRequestedIntegrationEventConsumer>();
            x.AddConsumer<ReleaseInventoryRequestedIntegrationEventConsumer>();

            x.AddEntityFrameworkOutbox<InventoryDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context, cfg) =>
            {
                var port = ushort.TryParse(configuration["RabbitMQ:Port"], out var configuredPort)
                    ? configuredPort
                    : (ushort)5672;

                cfg.Host(
                    configuration["RabbitMQ:Host"]!,
                    port,
                    configuration["RabbitMQ:VirtualHost"] ?? "/",
                    h =>
                    {
                        h.Username(
                            configuration["RabbitMQ:Username"]!);

                        h.Password(
                            configuration["RabbitMQ:Password"]!);
                    });
                cfg.UseMessageRetry(r =>
                {
                    r.Exponential(
                        retryLimit: 5,
                        minInterval: TimeSpan.FromSeconds(1),
                        maxInterval: TimeSpan.FromSeconds(30),
                        intervalDelta: TimeSpan.FromSeconds(5));
                });

                cfg.ReceiveEndpoint(
                    "inventory-reserve-requested",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            ReserveInventoryRequestedIntegrationEventConsumer>(
                            context);
                    });

                cfg.ReceiveEndpoint(
                    "inventory-release-requested",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            ReleaseInventoryRequestedIntegrationEventConsumer>(
                            context);
                    });
            });
        });


        return services;
    }
}
