using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class GroupsCreateRequirements : IAuthorizationRequirement
    {
    }

    public class GroupsCreateHandler : AbstractSiteLevelOrAboveClaimHandler<GroupsCreateRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public GroupsCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteGroupsCreate,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }
        
    }
}
