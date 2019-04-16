using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class UserProfileCreateRequirement : IAuthorizationRequirement
    {
    }

    public class UserProfileCreateHandler : AbstractSiteLevelOrAboveClaimHandler<UserProfileCreateRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public UserProfileCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteUsersCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
