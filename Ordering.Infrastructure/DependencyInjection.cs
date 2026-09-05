using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Interfaces;
using Ordering.Application.Services;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Messaging.Consumers;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;
using Ordering.Infrastructure.Saga;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {


            services.AddDbContext<OrderingDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        configuration
                        .GetConnectionString(
                        "OrderingConnection"));
                });
            services.AddDbContext<OrderSagaDbContext>(
                    options =>
                    {
                        options.UseSqlServer(
                                configuration
                                .GetConnectionString(
                                "OrderingConnection"));
                    });

            services.AddScoped<IOrderingDbContext>(
          provider =>
              provider.GetRequiredService<OrderingDbContext>());



            services.AddScoped<IOrderRepository,
                                OrderRepository>();



            services.AddScoped<IUnitOfWork,
                                UnitOfWork>();

            services.AddScoped<
             IDomainEventDispatcher,
              DomainEventDispatcher>();


            #region MassTransit
            services.AddMassTransit(x =>
            {

                x.AddSagaStateMachine<
                    OrderStateMachine,
                    OrderSagaState,
                    OrderStateMachineDefinition>()

                    .EntityFrameworkRepository(r =>
                    {
                    r.ExistingDbContext<OrderSagaDbContext>();

                    r.UseSqlServer();
                    });



                x.AddEntityFrameworkOutbox<OrderSagaDbContext>(o =>
                {
                    o.UseSqlServer();
                });





                x.AddConsumer<
                    InventoryReservedIntegrationEventConsumer>();
                x.AddConsumer<
                    InventoryReservationFailedIntegrationEventConsumer>();

                x.AddEntityFrameworkOutbox<OrderingDbContext>(o =>
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
                        "ordering-inventory-reserved",
                        endpoint =>
                        {
                            endpoint.ConfigureConsumer<
                                InventoryReservedIntegrationEventConsumer>(
                                context);
                        });


                    cfg.ReceiveEndpoint(
                        "ordering-order-saga",
                        endpoint =>
                        {
                            endpoint.ConfigureSaga<OrderSagaState>(
                                context);
                        });


                    cfg.ReceiveEndpoint(
                       "ordering-inventory-reservation-failed",
                       endpoint =>
                       {
                          endpoint.ConfigureConsumer<
                          InventoryReservationFailedIntegrationEventConsumer>(
                          context);
                       });
                });
            });
            #endregion




            return services;

        }
    }
}
