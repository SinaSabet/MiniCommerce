using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Payment.API.Extensions;
using Payment.API.Middleware;
using Payment.Application;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services
    .AddHealthChecks()

    .AddCheck(
        "self",
        () => HealthCheckResult.Healthy(),
        tags: new[] { "live" })

    .AddDbContextCheck<PaymentDbContext>(
        tags: new[] { "ready" });

 

builder.Services
    .AddApplication();


builder.Services
    .AddInfrastructure(
        builder.Configuration);

builder.Services
    .AddOpenTelemetryTracing();


Log.Logger =
    new LoggerConfiguration()
        .ReadFrom.Configuration(
            builder.Configuration)
        .CreateLogger();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host
    .UseSerilog();
var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = check =>
            check.Tags.Contains("live")
    });


app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = check =>
            check.Tags.Contains("ready")
    });

app.Run();
