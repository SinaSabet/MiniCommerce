using Payment.Application.Interfaces;
using Payment.Domain.PaymentTransactions;
using Payment.Infrastructure.Messaging.Consumers;
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

        // Register MassTransit
        services.AddMassTransit(x =>
        {
            x.AddConsumer<PaymentRequestedIntegrationEventConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var username = configuration["RabbitMQ:Username"] ?? "guest";
                var password = configuration["RabbitMQ:Password"] ?? "guest";

                cfg.Host(
                    host,
                    h =>
                    {
                        h.Username(username);
                        h.Password(password);
                    });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
