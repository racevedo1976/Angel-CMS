using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class SiteSettingsEditRequirement : IAuthorizationRequirement
    {
    }
    public class SiteSettingsEditHandler : AbstractSiteLevelOrAboveClaimHandler<SiteSettingsEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SiteSettingsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteSettingsEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
