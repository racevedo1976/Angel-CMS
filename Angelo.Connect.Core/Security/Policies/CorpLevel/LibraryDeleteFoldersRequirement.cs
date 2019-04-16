using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryDeleteFoldersRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryDeleteFoldersHandler : AbstractCorpLevelClaimHandler<LibraryDeleteFoldersRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryDeleteFoldersHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryDeleteFolders,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
