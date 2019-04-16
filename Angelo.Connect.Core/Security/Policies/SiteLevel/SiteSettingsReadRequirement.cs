using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class SiteSettingsReadRequirement : IAuthorizationRequirement
    {
    }

    public class SiteSettingsReadHandler : AbstractSiteLevelOrAboveClaimHandler<SiteSettingsReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SiteSettingsReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteSettingsRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
