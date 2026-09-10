using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MiniCommerce.Identity.Claims;
using MiniCommerce.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommerce.Identity.Authentication
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddMiniCommerceAuthentication(
      this IServiceCollection services,
      IConfiguration configuration)
        {
            services
            .AddOptions<KeycloakOptions>()
            .Bind(configuration.GetSection(KeycloakOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Authority),
                "Keycloak Authority is required.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Audience),
                "Keycloak Audience is required.")
            .ValidateOnStart();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var keycloakOptions = configuration
                        .GetSection(KeycloakOptions.SectionName)
                        .Get<KeycloakOptions>()
                        ?? throw new InvalidOperationException(
                            "Keycloak configuration is missing.");

                    options.Authority = keycloakOptions.Authority;
                    options.Audience = keycloakOptions.Audience;
                    options.RequireHttpsMetadata =
                        keycloakOptions.RequireHttpsMetadata;

                    options.MapInboundClaims = false;

                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,

                            NameClaimType =
                                CustomClaimTypes.PreferredUsername,

                            RoleClaimType =
                                CustomClaimTypes.Roles
                        };
                });

            return services;

        }
    }
}
