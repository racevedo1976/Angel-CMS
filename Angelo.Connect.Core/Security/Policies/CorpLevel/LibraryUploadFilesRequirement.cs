using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryUploadFilesRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryUploadFilesHandler : AbstractCorpLevelClaimHandler<LibraryUploadFilesRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryUploadFilesHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryUploadFiles,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
