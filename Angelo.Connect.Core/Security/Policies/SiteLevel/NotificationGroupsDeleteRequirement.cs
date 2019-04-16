using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NotificationGroupsDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class NotificationGroupsDeleteHandler : AbstractSiteLevelOrAboveClaimHandler<NotificationGroupsDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NotificationGroupsDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNotifyGroupsDelete,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
