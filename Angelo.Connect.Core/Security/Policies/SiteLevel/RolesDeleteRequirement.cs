using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class RolesDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class RolesDeleteHandler : AbstractClientLevelOrAboveClaimHandler<RolesDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteRolesDelete,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
