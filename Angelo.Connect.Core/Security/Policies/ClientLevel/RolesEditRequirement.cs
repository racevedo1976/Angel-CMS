using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class RolesEditRequirement : IAuthorizationRequirement
    {
    }
    public class RolesEditHandler : AbstractClientLevelOrAboveClaimHandler<RolesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public RolesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppRolesEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
