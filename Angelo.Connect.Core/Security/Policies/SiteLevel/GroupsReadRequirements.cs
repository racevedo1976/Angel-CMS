using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class GroupsReadRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsReadHandler : AbstractSiteLevelOrAboveClaimHandler<GroupsReadRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }
        public GroupsReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteGroupsRead,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
