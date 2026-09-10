using System.Net;
using System.Text.Json;

namespace Api.Gateway.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;


        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }



        public async Task InvokeAsync(
            HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. TraceId: {TraceId}",
                    context.TraceIdentifier);


                await HandleExceptionAsync(
                    context,
                    ex);

            }

        }



        private static async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {


            context.Response.ContentType =
                "application/json";


            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;



            var response = new
            {

                status = context.Response.StatusCode,

                title = "Internal Server Error",

                traceId = context.TraceIdentifier,

                message =
                    "An unexpected error occurred."

            };



            await context.Response.WriteAsync(

                JsonSerializer.Serialize(response)

            );

        }

    }
}
