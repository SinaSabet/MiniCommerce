using Payment.Application.Interfaces;
using Payment.Domain.PaymentTransactions;
using Payment.Infrastructure.Messaging.Consumers;
using Payment.Infrastructure.Messaging.Observers;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Payment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("PaymentConnection"));
        });

        services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddReceiveObserver<ServiceFaultReceiveObserver>();

        // Register MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<PaymentRequestedIntegrationEventConsumer>();
            x.AddConsumer<RefundPaymentRequestedIntegrationEventConsumer>();

            x.AddEntityFrameworkOutbox<PaymentDbContext>(o =>
            {
                o.UseSqlServer();
                o.UseBusOutbox();
            });
            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var port = ushort.TryParse(configuration["RabbitMQ:Port"], out var configuredPort)
                    ? configuredPort
                    : (ushort)5672;
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(
                    host,
                    port,
                    configuration["RabbitMQ:VirtualHost"] ?? "/",
                    h =>
                    {
                        h.Username(username);
                        h.Password(password);
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
                    "payment-requested",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            PaymentRequestedIntegrationEventConsumer>(
                            context);
                    });

                cfg.ReceiveEndpoint(
                    "payment-refund-requested",
                    endpoint =>
                    {
                        endpoint.ConfigureConsumer<
                            RefundPaymentRequestedIntegrationEventConsumer>(
                            context);
                    });
            });
        });

        return services;
    }
}
