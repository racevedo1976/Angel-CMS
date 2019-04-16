using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class SiteTemplatesReadRequirement : IAuthorizationRequirement
    {
    }

    public class SiteTemplatesReadHandler : AbstractSiteLevelOrAboveClaimHandler<SiteTemplatesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SiteTemplatesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteTemplateRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
