using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class RolesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class RolesCreateHandler : AbstractSiteLevelOrAboveClaimHandler<RolesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteRolesCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
