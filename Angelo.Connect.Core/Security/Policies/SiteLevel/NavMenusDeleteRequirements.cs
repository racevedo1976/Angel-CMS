using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NavMenusDeleteRequirement : IAuthorizationRequirement
    {
    }
    public class NavMenusDeleteHandler : AbstractSiteLevelOrAboveClaimHandler<NavMenusDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NavMenusDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNavMenusDelete,
                SiteClaimTypes.SiteGroupsDelete,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
