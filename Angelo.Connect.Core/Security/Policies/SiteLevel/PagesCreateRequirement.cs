using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class PagesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class PagesCreateHandler : AbstractSiteLevelOrAboveClaimHandler<PagesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public PagesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePagesCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
