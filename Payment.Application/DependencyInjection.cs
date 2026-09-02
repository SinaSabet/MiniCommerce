using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Behaviors;
using Payment.Application.EventHandlers;
using Payment.Application.Interfaces;
using Payment.Domain.DomainEvents;

namespace Payment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(
                cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(DependencyInjection)
                    .Assembly));

        services.AddTransient(
           typeof(IPipelineBehavior<,>),
           typeof(ValidationBehavior<,>));

        services.AddTransient(
         typeof(IPipelineBehavior<,>),
         typeof(TransactionBehavior<,>));

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection)
            .Assembly);

        // Register domain event handlers
        services.AddScoped<
            IDomainEventHandler<PaymentCompletedDomainEvent>,
            PaymentCompletedEventHandler>();

        services.AddScoped<
            IDomainEventHandler<PaymentFailedDomainEvent>,
            PaymentFailedEventHandler>();

        return services;
    }
}
