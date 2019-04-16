using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class NotificationGroupsEditRequirement : IAuthorizationRequirement
    {
    }

    public class NotificationGroupsEditHandler : AbstractClientLevelOrAboveClaimHandler<NotificationGroupsEditRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NotificationGroupsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppNotifyGroupsEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
