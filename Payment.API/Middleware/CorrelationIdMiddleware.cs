namespace Payment.API.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;


        public CorrelationIdMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }



        public async Task InvokeAsync(
            HttpContext context)
        {

            var correlationId =
                context.Request.Headers["X-Correlation-ID"]
                .FirstOrDefault()
                ?? Guid.NewGuid().ToString();



            context.Response.Headers
                .Append(
                    "X-Correlation-ID",
                    correlationId);



            using (
                Serilog.Context.LogContext.PushProperty(
                    "CorrelationId",
                    correlationId))
            {

                await _next(context);

            }

        }
    }
}
