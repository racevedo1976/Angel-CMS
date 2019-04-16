using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class GroupsAssignRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsAssignHandler : AbstractClientLevelOrAboveClaimHandler<GroupsAssignRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public GroupsAssignHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppGroupsAssign,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
