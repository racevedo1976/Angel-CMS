using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ProductsReadRequirements : IAuthorizationRequirement
    {
    }

    public class ProductsReadHandler : AbstractCorpLevelClaimHandler<ProductsReadRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ProductsReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpProductsRead,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
