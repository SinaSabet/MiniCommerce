using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Application.Interfaces;
using Ordering.Application.Services;
using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Persistence;
using Ordering.Infrastructure.Persistence.Repositories;
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
                x.AddEntityFrameworkOutbox<OrderingDbContext>(o =>
                {
                    o.UseSqlServer();

                    o.UseBusOutbox();
                });


                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(
                        "localhost",
                        "/",
                        h =>
                        {
                            h.Username("admin");
                            h.Password("admin123");
                        });

                    cfg.ConfigureEndpoints(context);
                });
            });
            #endregion




            return services;

        }
    }
}
