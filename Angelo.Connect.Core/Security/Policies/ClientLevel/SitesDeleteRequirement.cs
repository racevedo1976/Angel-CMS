using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class SitesDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class SitesDeleteHandler : AbstractClientLevelOrAboveClaimHandler<SitesDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SitesDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.SitesDelete,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
