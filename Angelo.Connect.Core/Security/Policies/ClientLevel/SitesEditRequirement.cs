using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class SitesEditRequirement : IAuthorizationRequirement
    {
    }

    public class SitesEditHandler : AbstractClientLevelOrAboveClaimHandler<SitesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SitesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.SitesEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
