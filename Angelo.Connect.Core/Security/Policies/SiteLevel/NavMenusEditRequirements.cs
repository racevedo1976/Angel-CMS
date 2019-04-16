using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NavMenusEditRequirement : IAuthorizationRequirement
    {
    }

    public class NavMenusEditHandler : AbstractSiteLevelOrAboveClaimHandler<NavMenusEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NavMenusEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNavMenusEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
