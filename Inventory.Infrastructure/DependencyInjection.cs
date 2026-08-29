using Inventory.Application.Interfaces;
using Inventory.Domain.InventoryItems;
using Inventory.Domain.Reservations;
using Inventory.Infrastructure.Messaging.Consumers;
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


        services.AddMassTransit(x =>
        {
            x.AddConsumer<
                OrderConfirmedIntegrationEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(
                    configuration["RabbitMQ:Host"]!,
                    configuration["RabbitMQ:VirtualHost"] ?? "/",
                    h =>
                    {
                        h.Username(
                            configuration["RabbitMQ:Username"]!);

                        h.Password(
                            configuration["RabbitMQ:Password"]!);
                    });


                cfg.ReceiveEndpoint(
                    "inventory-order-confirmed",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            OrderConfirmedIntegrationEventConsumer>(
                            context);
                    });
            });
        });


        return services;
    }
}