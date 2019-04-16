using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

using Angelo.Identity.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Angelo.Connect.Security.Policies
{
    public abstract class AbstractCorpLevelClaimHandler<TRequirement> : AuthorizationHandler<TRequirement>
    where TRequirement : IAuthorizationRequirement
    {
        private IContextAccessor<AdminContext> _adminContextAccessor;

        public abstract IEnumerable<string> ValidClaimTypes { get; set; }

        public AbstractCorpLevelClaimHandler(IContextAccessor<AdminContext> adminContextAccessor)
        {
            _adminContextAccessor = adminContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TRequirement requirement)
        {

            if (ValidClaimTypes != null && ValidClaimTypes.Count() > 0)
            {
                var adminContext = _adminContextAccessor.GetContext();
                var userContext = adminContext.UserContext;

                // Build Claims with Corp values since any would be valid
                var validClaims = ValidClaimTypes.SelectMany(type => new Claim[]
                {
                    new Claim(type, adminContext.CorpId)
                });

                // Aegis does not return permission level claims to keep the ticket small, etc.
                // Instead, these are loaded locally into the UserContext
                var hasAtLeastOneClaim = userContext.SecurityClaims.Any(
                    userClaim => validClaims.Any(
                        validClaim => userClaim.Type == validClaim.Type && userClaim.Value == validClaim.Value
                    )
                );

                if (hasAtLeastOneClaim)
                {
                    context.Succeed(requirement);
                }

            }

            return Task.CompletedTask;
        }
    }
}
