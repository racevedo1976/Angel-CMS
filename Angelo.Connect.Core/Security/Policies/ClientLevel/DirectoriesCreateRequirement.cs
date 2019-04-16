using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class DirectoriesCreateRequirement : IAuthorizationRequirement
    {
    }

    public class DirectoriesCreateHandler : AbstractClientLevelOrAboveClaimHandler<DirectoriesCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public DirectoriesCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UserDirectoryCreate,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
