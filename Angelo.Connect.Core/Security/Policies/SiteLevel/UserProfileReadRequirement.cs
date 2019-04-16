using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class UserProfileReadRequirement : IAuthorizationRequirement
    {
    }

    public class UserProfileReadHandler : AbstractSiteLevelOrAboveClaimHandler<UserProfileReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public UserProfileReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteUsersRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
    }
}
