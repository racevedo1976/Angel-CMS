using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Aegis.Internal
{
    public class IdentityProfileService : IProfileService
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager _userManager;
        private readonly ClaimsFactory _claimsFactory;
        private readonly SecurityPoolManager _poolManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AegisTenantResolver _tenantResolver;
        private readonly SignInManager _signInManager;

        public IdentityProfileService
        (
            IIdentityServerInteractionService interaction, 
            UserManager userManager, 
            ClaimsFactory claimsFactory, 
            SecurityPoolManager poolManager,
            SignInManager signInManager,
            IHttpContextAccessor contextAccessor,
            AegisTenantResolver tenantResolver
        )
        {
            _interaction = interaction;
            _userManager = userManager;
            _claimsFactory = claimsFactory;
            _poolManager = poolManager;
            _signInManager = signInManager;

            _httpContextAccessor = contextAccessor;
            _tenantResolver = tenantResolver;
        }

        async public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var tenantContext = await _tenantResolver.ResolveAsync(httpContext);
            var tenant = tenantContext.Tenant;

            //var poolId = GetRequestedPoolId();

            if (context.RequestedClaimTypes.Any())
            {           
                var userId = context.Subject.GetSubjectId();                
                var user = await _userManager.FindByIdAsync(userId);

                var validTenantUser = await _signInManager.IsValidTenantUser(tenant.TenantId, user);

                if (validTenantUser)
                {                
                    var principal = await _claimsFactory.CreateAsync(user);

                    var issuedClaims = principal.Claims
                        .Where(claim => context.RequestedClaimTypes.Contains(claim.Type))
                        .ToList();

                    context.IssuedClaims.AddRange(issuedClaims);          
                }              
            }  
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userId = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(userId);

            context.IsActive = user != null && user.IsActive;
        }
    }
}
