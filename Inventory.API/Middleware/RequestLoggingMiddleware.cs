using Serilog;

namespace Inventory.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;


        public RequestLoggingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }



        public async Task InvokeAsync(
            HttpContext context)
        {

            var start =
                DateTime.UtcNow;



            Log.Information(
                "HTTP Request Started {Method} {Path}",
                context.Request.Method,
                context.Request.Path);



            await _next(context);



            var duration =
                DateTime.UtcNow - start;



            Log.Information(
                "HTTP Request Finished {Method} {Path} {StatusCode} {Duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds);

        }
    }

}
