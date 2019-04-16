using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class NotificationGroupsReadRequirement : IAuthorizationRequirement
    {
    }

    public class NotificationGroupsReadHandler : AbstractClientLevelOrAboveClaimHandler<NotificationGroupsReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NotificationGroupsReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteNotifyGroupsRead,
                ClientClaimTypes.PrimaryAdmin,
                SiteClaimTypes.SitePrimaryAdmin
            };
        }

    }
}
