using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class DirectoriesEditRequirement : IAuthorizationRequirement
    {
    }

    public class DirectoriesEditHandler : AbstractClientLevelOrAboveClaimHandler<DirectoriesEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public DirectoriesEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UserDirectoryEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
