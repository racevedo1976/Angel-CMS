using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class GroupsAssignRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsAssignHandler : AbstractSiteLevelOrAboveClaimHandler<GroupsAssignRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }
        public GroupsAssignHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteGroupsAssign,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
