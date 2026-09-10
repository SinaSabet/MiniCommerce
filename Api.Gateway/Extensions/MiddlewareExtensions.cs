using Api.Gateway.Middleware;

namespace Api.Gateway.Extensions
{
    public static class MiddlewareExtensions
    {

        public static IApplicationBuilder
            UseCorrelationId(
            this IApplicationBuilder app)
        {

            return app.UseMiddleware<Api.Gateway.Middleware.CorrelationIdMiddleware>();

        }
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {

            return app.UseMiddleware<ExceptionHandlingMiddleware>();

        }
    }
}
