using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class LibraryOwnerRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryOwnerHandler : AbstractSiteLevelOrAboveClaimHandler<LibraryOwnerRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryOwnerHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteLibraryOwner,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
