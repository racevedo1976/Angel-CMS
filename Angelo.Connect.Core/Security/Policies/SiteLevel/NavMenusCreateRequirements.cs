using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NavMenusCreateRequirement : IAuthorizationRequirement
    {
    }

    public class NavMenusCreateHandler : AbstractSiteLevelOrAboveClaimHandler<NavMenusCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NavMenusCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNavMenusCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
