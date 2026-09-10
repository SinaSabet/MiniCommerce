using Microsoft.Extensions.DependencyInjection;

namespace MiniCommerce.Identity.Authorization;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddMiniCommerceAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                Policies.Customer,
                policy => policy.RequireRole("customer"));

            options.AddPolicy(
                Policies.Admin,
                policy => policy.RequireRole("admin"));

            options.AddPolicy(
                Policies.WarehouseManager,
                policy => policy.RequireRole("warehouse-manager"));

            options.AddPolicy(
                Policies.PaymentOperator,
                policy => policy.RequireRole("payment-operator"));
        });

        return services;
    }
}