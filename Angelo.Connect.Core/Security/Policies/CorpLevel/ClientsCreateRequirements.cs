using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ClientsCreateRequirements : IAuthorizationRequirement
    {
    }

    public class ClientsCreateHandler : AbstractCorpLevelClaimHandler<ClientsCreateRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ClientsCreateHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpCustomersCreate,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
