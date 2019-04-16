using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class AnyAdminClaimToSiteRequirement : IAuthorizationRequirement
    {
    }

    public class AnyAdminClaimToSiteHandler : AuthorizationHandler<AnyAdminClaimToSiteRequirement, string>
    {
        IContextAccessor<UserContext> _userContextAccessor;

        public AnyAdminClaimToSiteHandler(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

       
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyAdminClaimToSiteRequirement requirement, string siteId)
        {
            var claims = _userContextAccessor.GetContext()?.SecurityClaims;
           

            if (siteId == null)
                throw new NullReferenceException("Site Id is required to evaluate policy.");


            if (claims != null)
            {
                // NOTE: This assumes all current and future site claim types follow standard naming convention 
                //       since future claims introduced by plugins are not yet known, but also need to be considered 
                //       for the overall the success of this requirement.
                //
                //       Taking this route now because it saves time and is an "okay" approach and still very secure.

                // TODO: Best practice approach would be to explicitly list out known core claims here, then have plugins 
                //       register their own AuthorizationHandler<> targeting the above "AnySiteLevelClaimRequirement"
                //       to extend success logic for the specific claims they've introduced


                var hasAnyClientClaim = claims.Any(x =>
                        x.Type.StartsWith("site-", StringComparison.OrdinalIgnoreCase)
                        && ( x.Value == siteId )
                );

                if (hasAnyClientClaim)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}


