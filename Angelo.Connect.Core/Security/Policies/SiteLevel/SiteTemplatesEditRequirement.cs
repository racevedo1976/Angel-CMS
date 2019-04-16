using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class SiteTemplatesEditRequirement : IAuthorizationRequirement
    {
    }

    public class SiteTemplatesEditHandler : AbstractSiteLevelOrAboveClaimHandler<SiteTemplatesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SiteTemplatesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteTemplateEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
    