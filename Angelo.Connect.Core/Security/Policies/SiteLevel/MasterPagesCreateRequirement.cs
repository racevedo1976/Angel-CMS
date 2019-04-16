using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class MasterPagesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class MasterPagesCreateHandler : AbstractSiteLevelOrAboveClaimHandler<MasterPagesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public MasterPagesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteMasterPagesCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
