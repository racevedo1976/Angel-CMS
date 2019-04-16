using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class ProductsDeleteRequirements : IAuthorizationRequirement
    {
    }

    public class ProductsDeleteHandler : AbstractCorpLevelClaimHandler<ProductsDeleteRequirements>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public ProductsDeleteHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpProductsDelete,
                CorpClaimTypes.CorpPrimaryAdmin
            };
        }
        
    }
}
