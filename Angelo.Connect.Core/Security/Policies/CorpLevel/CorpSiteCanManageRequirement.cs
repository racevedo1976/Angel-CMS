using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class CorpSiteCanManageRequirement : IAuthorizationRequirement
    {
    }

    public class CorpSiteCanManageHandler : AbstractCorpLevelClaimHandler<CorpSiteCanManageRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public CorpSiteCanManageHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpSitesCreate,
                CorpClaimTypes.CorpSitesEdit,
                CorpClaimTypes.CorpSitesDelete,
                CorpClaimTypes.CorpSitesRead,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
    }
}
