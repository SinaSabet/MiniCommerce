using Ordering.Domain.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace Ordering.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
        HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (DomainException ex)
            {

                context.Response.StatusCode =
                    StatusCodes.Status400BadRequest;


                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        message = ex.Message
                    });

            }
            catch (Exception ex)
            {

                context.Response.StatusCode =
                    StatusCodes.Status500InternalServerError;


                await context.Response.WriteAsJsonAsync(
                    new
                    {
                        message =
                        "Internal Server Error"
                    });

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



            var response =
                new
                {
                    statusCode =
                        context.Response.StatusCode,

                    message =
                        exception.Message
                };



            await context.Response
                .WriteAsync(
                    JsonSerializer.Serialize(response));

        }

    }
}
