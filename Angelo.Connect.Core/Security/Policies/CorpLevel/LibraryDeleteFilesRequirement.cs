using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryDeleteFilesRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryDeleteFilesHandler : AbstractCorpLevelClaimHandler<LibraryDeleteFilesRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryDeleteFilesHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryDeleteFiles,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
