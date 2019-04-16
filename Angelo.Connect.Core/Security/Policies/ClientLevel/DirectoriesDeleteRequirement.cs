using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class DirectoriesDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class DirectoriesDeleteHandler : AbstractClientLevelOrAboveClaimHandler<DirectoriesDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public DirectoriesDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UserDirectoryDelete,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
