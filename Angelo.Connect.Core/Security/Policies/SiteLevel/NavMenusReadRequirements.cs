using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NavMenusReadRequirement : IAuthorizationRequirement
    {
    }

    public class NavMenusReadHandler : AbstractSiteLevelOrAboveClaimHandler<NavMenusReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }
        public NavMenusReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNavMenusRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
