using Microsoft.AspNetCore.Mvc;
using Shipping.Domain.Common.Exceptions;

namespace Shipping.API.Middleware;

public sealed class ExceptionHandlingMiddleware
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

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteProblemDetailsAsync(context, exception);
        }
    }

    private async Task WriteProblemDetailsAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            DomainException domainException =>
                (StatusCodes.Status400BadRequest,
                    "Domain validation failed",
                    domainException.Message),
            KeyNotFoundException notFoundException =>
                (StatusCodes.Status404NotFound,
                    "Resource not found",
                    notFoundException.Message),
            _ =>
                (StatusCodes.Status500InternalServerError,
                    "Internal server error",
                    "An unexpected error occurred.")
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled API exception. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "API request rejected. TraceId: {TraceId}, StatusCode: {StatusCode}",
                context.TraceIdentifier,
                statusCode);
        }

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };

        problem.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problem,
            context.RequestAborted);
    }
}
