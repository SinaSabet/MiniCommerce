using Api.Gateway.Configuration;
using Api.Gateway.Extensions;
using MiniCommerce.Identity.Authentication;
using MiniCommerce.Identity.Authorization;

var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddReverseProxyConfiguration(
        builder.Configuration
    );

builder.Services
    .AddGatewayHealthChecks();


builder.Services
    .AddGatewayObservability();

builder.Services
    .AddMiniCommerceAuthentication(builder.Configuration);

builder.Services
    .AddMiniCommerceAuthorization();

var app = builder.Build();


app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseCorrelationId();

app.UseAuthentication();

app.UseAuthorization();

app.UseGatewayHealthChecks();

app.UseGatewayPipeline();


app.Run();

