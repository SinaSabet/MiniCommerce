namespace Api.Gateway.Extensions
{
    public static class ApplicationExtensions
    {
        public static WebApplication UseGatewayPipeline(this WebApplication app) 
        {
            app.MapReverseProxy();


            return app;
        }
    }
}
