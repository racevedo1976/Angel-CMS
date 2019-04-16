using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class LibraryManageSecurityRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryManageSecurityHandler : AbstractCorpLevelClaimHandler<LibraryManageSecurityRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryManageSecurityHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpLibraryManageSecurity,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }

    }
}
