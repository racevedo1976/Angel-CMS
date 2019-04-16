using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class MasterPagesEditRequirement : IAuthorizationRequirement
    {
    }

    public class MasterPagesEditHandler : AbstractSiteLevelOrAboveClaimHandler<MasterPagesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public MasterPagesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteMasterPagesEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
