using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class SitesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class SitesCreateHandler : AbstractClientLevelOrAboveClaimHandler<SitesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public SitesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.SitesCreate,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
