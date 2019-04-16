using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.ClientLevel
{
    public class AnyAdminClaimRequirement : IAuthorizationRequirement
    {
    }

    public class AnyAdminClaimCoreHandler : AuthorizationHandler<AnyAdminClaimRequirement>
    {

        private IContextAccessor<UserContext> _userContextAccessor;
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;

        public AnyAdminClaimCoreHandler(IContextAccessor<UserContext> userContextAccessor, IContextAccessor<ClientAdminContext> clientContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
            _clientContextAccessor = clientContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyAdminClaimRequirement requirement)
        {
            var claims = _userContextAccessor.GetContext()?.SecurityClaims;
            var client = _clientContextAccessor.GetContext()?.Client;

            if (client == null)
                throw new NullReferenceException("ClientAdminContext.Client is required to evaluate policy.");


            if (claims != null)
            {
                // NOTE: This assumes all current and future client claim types follow standard naming convention 
                //       since future claims introduced by plugins are not yet known, but also need to be considered 
                //       for the overall the success of this requirement.
                //
                //       Taking this route now because it saves time and is an "okay" approach and still very secure.

                // TODO: Best practice approach would be to explicitly list out known core claims here, then have plugins 
                //       register their own AuthorizationHandler<> targeting the above "AnyClientLevelClaimRequirement"
                //       to extend success logic for the specific claims they've introduced


                var hasAnyClientClaim = claims.Any(x =>
                    x.Type.StartsWith("client-", StringComparison.OrdinalIgnoreCase)
                    && (
                        x.Value == client.Id
                        || x.Value == ConnectCoreConstants.CorporateId
                    )
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
