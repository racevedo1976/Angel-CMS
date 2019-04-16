using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class PagesDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class PagesDeleteHandler : AbstractSiteLevelOrAboveClaimHandler<PagesDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public PagesDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePagesDelete,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }

}
