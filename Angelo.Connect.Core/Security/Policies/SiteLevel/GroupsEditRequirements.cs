using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class GroupsEditRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsEditHandler : AbstractSiteLevelOrAboveClaimHandler<GroupsEditRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public GroupsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteGroupsEdit,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
