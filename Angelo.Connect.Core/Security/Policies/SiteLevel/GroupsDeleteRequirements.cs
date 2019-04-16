using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class GroupsDeleteRequirements : IAuthorizationRequirement
    {
    }
    public class GroupsDeleteHandler : AbstractSiteLevelOrAboveClaimHandler<GroupsDeleteRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public GroupsDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SitePrimaryAdmin,
                SiteClaimTypes.SiteGroupsDelete,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
