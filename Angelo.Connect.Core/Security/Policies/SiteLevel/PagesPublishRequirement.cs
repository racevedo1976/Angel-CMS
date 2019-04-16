using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class PagesPublishRequirement : IAuthorizationRequirement
    {
    }
    public class PagesPublishHandler : AbstractSiteLevelOrAboveClaimHandler<PagesPublishRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public PagesPublishHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePagesPublish,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
