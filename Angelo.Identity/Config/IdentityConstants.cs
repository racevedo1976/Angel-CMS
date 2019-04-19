using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Config
{
    public static class IdentityConstants
    {
        public static string IdentityProvider { get; } = "aegis";

        public static string CorpTenantKey { get; } = "MyCompany";

        public static string CorpDirectoryId { get; } = "dbacd2ad1c8b43c8b6038d0453847b8c";
        
        public static string UserIdClaimType { get; } = ClaimType.Subject;
        public static string UserNameClaimType { get; } = ClaimType.Name;
        public static string RoleClaimType { get; } = ClaimType.Role;
    }
}
