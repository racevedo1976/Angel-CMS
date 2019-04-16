using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class SitesReadRequirement : IAuthorizationRequirement
    {
    }

    public class SitesReadHandler : AbstractClientLevelOrAboveClaimHandler<SitesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SitesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.SitesRead,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
