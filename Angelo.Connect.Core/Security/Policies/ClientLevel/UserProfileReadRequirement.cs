using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class UserProfileReadRequirement : IAuthorizationRequirement
    {
    }

    public class UserProfileReadHandler : AbstractClientLevelOrAboveClaimHandler<UserProfileReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public UserProfileReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UsersRead,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
