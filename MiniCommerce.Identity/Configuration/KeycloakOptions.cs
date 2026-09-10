using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommerce.Identity.Configuration
{
    public sealed class KeycloakOptions
    {
        public const string SectionName = "Keycloak";

        public string Authority { get; init; } = string.Empty;

        public string Audience { get; init; } = string.Empty;

        public bool RequireHttpsMetadata { get; init; } = true;
    }
}
