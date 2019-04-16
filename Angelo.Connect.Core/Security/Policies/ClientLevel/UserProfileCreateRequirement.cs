using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class UserProfileCreateRequirement : IAuthorizationRequirement
    {
    }

    public class UserProfileCreateHandler : AbstractClientLevelOrAboveClaimHandler<UserProfileCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public UserProfileCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.UsersCreate,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
