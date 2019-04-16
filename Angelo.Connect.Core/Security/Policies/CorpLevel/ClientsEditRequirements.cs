using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ClientsEditRequirements : IAuthorizationRequirement
    {
    }

    public class ClientsEditHandler : AbstractCorpLevelClaimHandler<ClientsEditRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ClientsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpCustomersEdit,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
