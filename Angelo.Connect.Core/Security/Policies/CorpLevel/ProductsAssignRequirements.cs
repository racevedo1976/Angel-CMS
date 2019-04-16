using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ProductsAssignRequirements : IAuthorizationRequirement
    {
    }

    public class ProductsAssignHandler : AbstractCorpLevelClaimHandler<ProductsAssignRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ProductsAssignHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpProductsAssign,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
