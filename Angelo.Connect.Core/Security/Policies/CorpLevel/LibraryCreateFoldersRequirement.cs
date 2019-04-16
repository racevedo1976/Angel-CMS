using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryCreateFoldersRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryCreateFoldersHandler : AbstractCorpLevelClaimHandler<LibraryCreateFoldersRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryCreateFoldersHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryCreateFolders,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
