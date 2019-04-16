using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NotificationGroupsEditRequirement : IAuthorizationRequirement
    {
    }

    public class NotificationGroupsEditHandler : AbstractSiteLevelOrAboveClaimHandler<NotificationGroupsEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NotificationGroupsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNotifyGroupsEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
