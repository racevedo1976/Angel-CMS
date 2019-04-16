using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class PagesDesignRequirement : IAuthorizationRequirement
    {
    }
    public class PagesDesignHandler : AbstractSiteLevelOrAboveClaimHandler<PagesDesignRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public PagesDesignHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePagesDesign,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
