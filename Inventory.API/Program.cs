using Inventory.API.Middleware;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<InventoryDbContext>()
    .AddRabbitMQ();

builder.Services.AddControllers();
builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

Log.Logger =
    new LoggerConfiguration()
        .ReadFrom.Configuration(
            builder.Configuration)
        .CreateLogger();


builder.Host
    .UseSerilog();
var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();


app.Run();