using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class UserProfileEditRequirement : IAuthorizationRequirement
    {
    }

    public class UserProfileEditHandler : AbstractClientLevelOrAboveClaimHandler<UserProfileEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public UserProfileEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UsersEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
