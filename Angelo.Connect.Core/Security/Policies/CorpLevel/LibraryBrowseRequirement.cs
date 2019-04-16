using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryBrowseRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryBrowseHandler : AbstractCorpLevelClaimHandler<LibraryBrowseRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryBrowseHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryBrowse,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
