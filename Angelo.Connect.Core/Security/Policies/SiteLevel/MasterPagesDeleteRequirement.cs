using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class MasterPagesDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class MasterPagesDeleteHandler : AbstractSiteLevelOrAboveClaimHandler<MasterPagesDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public MasterPagesDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteMasterPagesDelete,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
