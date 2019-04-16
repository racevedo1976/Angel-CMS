using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class DirectoriesReadRequirement : IAuthorizationRequirement
    {
    }

    public class DirectoriesReadHandler : AbstractClientLevelOrAboveClaimHandler<DirectoriesReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public DirectoriesReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UserDirectoryRead,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
