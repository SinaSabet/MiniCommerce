namespace Api.Gateway.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddGatewayHealthChecks(
        this IServiceCollection services)
        {

            services.AddHealthChecks();

            return services;
        }



        public static WebApplication UseGatewayHealthChecks(
            this WebApplication app)
        {


            app.MapHealthChecks("/health/live");


            app.MapHealthChecks("/health/ready");


            return app;

        }
    }
}
