using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ProductsEditRequirements : IAuthorizationRequirement
    {
    }

    public class ProductsEditHandler : AbstractCorpLevelClaimHandler<ProductsEditRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ProductsEditHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpProductsEdit,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
