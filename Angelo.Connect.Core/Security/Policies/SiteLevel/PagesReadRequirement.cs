using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class PagesReadRequirement : IAuthorizationRequirement
    {
    }
    public class PagesReadHandler : AbstractSiteLevelOrAboveClaimHandler<PagesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public PagesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePagesRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
