using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class RolesReadRequirement : IAuthorizationRequirement
    {
    }
    public class RolesReadHandler : AbstractSiteLevelOrAboveClaimHandler<RolesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteRolesRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
