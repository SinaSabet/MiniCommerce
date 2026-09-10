namespace Api.Gateway.Middleware
{

    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-ID";

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
                context.Request.Headers.ContainsKey(HeaderName)
                ?
                context.Request.Headers[HeaderName].ToString()
                :
                Guid.NewGuid().ToString();


            context.Response.Headers[HeaderName]
                = correlationId;


            context.Items[HeaderName]
                = correlationId;


            context.Request.Headers[HeaderName]
                = correlationId;


            await _next(context);

        }
    }
}