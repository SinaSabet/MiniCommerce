using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Behaviors;
using Ordering.Application.EventHandlers;
using Ordering.Application.Interfaces;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application
{
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

             services.AddScoped<
               IDomainEventHandler<OrderCreatedEvent>,
               OrderCreatedEventHandler>();


            services.AddScoped<
                 IDomainEventHandler<OrderPaidEvent>,
                 OrderPaidEventHandler>();


            return services;

        }
    }
}
