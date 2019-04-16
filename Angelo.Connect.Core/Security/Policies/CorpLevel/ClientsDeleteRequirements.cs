using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ClientsDeleteRequirements : IAuthorizationRequirement
    {
    }

    public class ClientsDeleteHandler : AbstractCorpLevelClaimHandler<ClientsDeleteRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ClientsDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpCustomersDelete,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
