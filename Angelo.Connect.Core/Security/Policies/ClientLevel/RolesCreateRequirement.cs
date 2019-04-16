using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class RolesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class RolesCreateHandler : AbstractClientLevelOrAboveClaimHandler<RolesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppRolesCreate,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
