using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryEditFoldersRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryEditFoldersHandler : AbstractCorpLevelClaimHandler<LibraryEditFoldersRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryEditFoldersHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryEditFolders,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
