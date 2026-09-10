using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCommerce.Identity.Claims
{
    public sealed class CustomClaimTypes
    {
        public const string Subject = "sub";

        public const string Email = "email";

        public const string PreferredUsername = "preferred_username";

        public const string Roles = "roles";
    }
}
