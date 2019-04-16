using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class RolesEditRequirement : IAuthorizationRequirement
    {
    }
    public class RolesEditHandler : AbstractSiteLevelOrAboveClaimHandler<RolesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteRolesEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
