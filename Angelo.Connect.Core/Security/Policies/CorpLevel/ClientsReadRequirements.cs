using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ClientsReadRequirements : IAuthorizationRequirement
    {
    }

    public class ClientsReadHandler : AbstractCorpLevelClaimHandler<ClientsReadRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ClientsReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpCustomersRead,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
