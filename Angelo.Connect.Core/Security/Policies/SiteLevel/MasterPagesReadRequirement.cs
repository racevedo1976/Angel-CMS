using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class MasterPagesReadRequirement : IAuthorizationRequirement
    {
    }

    public class MasterPagesReadHandler : AbstractSiteLevelOrAboveClaimHandler<MasterPagesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public MasterPagesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteMasterPagesRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
