using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class GroupsEditRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsEditHandler : AbstractClientLevelOrAboveClaimHandler<GroupsEditRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public GroupsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                ClientClaimTypes.AppGroupsEdit,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
