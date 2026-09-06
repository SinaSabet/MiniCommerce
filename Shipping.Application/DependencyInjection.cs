using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Shipping.Application.Behaviors;
using Shipping.Application.EventHandlers;
using Shipping.Application.Interfaces;
using Shipping.Domain.DomainEvents;

namespace Shipping.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(
                typeof(DependencyInjection).Assembly));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(TransactionBehavior<,>));

        services.AddScoped<
            IDomainEventHandler<ShipmentCreatedDomainEvent>,
            ShipmentCreatedDomainEventHandler>();

        services.AddScoped<
            IDomainEventHandler<ShipmentFailedDomainEvent>,
            ShipmentFailedDomainEventHandler>();

        return services;
    }
}
