using Inventory.Application.Behaviors;
using Inventory.Application.EventHandlers;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainEvents;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application;

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

        services.AddScoped<
            IDomainEventHandler<InventoryReservedDomainEvent>,
            InventoryReservedDomainEventHandler>();

        //services.AddValidatorsFromAssembly(
        //    typeof(DependencyInjection)
        //    .Assembly);


        return services;
    }
}