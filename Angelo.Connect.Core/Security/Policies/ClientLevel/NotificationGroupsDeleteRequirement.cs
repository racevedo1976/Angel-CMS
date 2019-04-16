using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class NotificationGroupsDeleteRequirement : IAuthorizationRequirement
    {
    }

    public class NotificationGroupsDeleteHandler : AbstractClientLevelOrAboveClaimHandler<NotificationGroupsDeleteRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public NotificationGroupsDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppNotifyGroupsDelete,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
