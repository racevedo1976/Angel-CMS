using System;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Identity.Config;
using Angelo.Aegis.Internal;

namespace Angelo.Aegis.Configuration
{
    public class AegisTenantIdentityOptions : IdentityOptions
    {
        public AegisTenantIdentityOptions(AegisTenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor) : base()
        {
            if (tenantResolver == null)
                throw new ArgumentNullException(nameof(tenantResolver));

            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            var tenantContext = tenantResolver.ResolveAsync(httpContextAccessor.HttpContext).Result;
            
            if (tenantContext?.Tenant != null)
            {
                Initialize(tenantContext.Tenant);
            }
        }

        private void Initialize(AegisTenant tenant)
        {
        
            this.Cookies = new IdentityCookieOptions
            {
                ApplicationCookie = new CookieAuthenticationOptions
                {
                    AuthenticationScheme = tenant.AuthSchemeInternal,
                    CookieName = tenant.AuthSchemeInternal,
                    SlidingExpiration = tenant.CookieSlidingExpiration,
                    ExpireTimeSpan = tenant.CookieLifetime
                },
                ExternalCookie = new CookieAuthenticationOptions
                {
                    AuthenticationScheme = tenant.AuthSchemeExternal,
                    CookieName = tenant.AuthSchemeExternal,
                    SlidingExpiration = tenant.CookieSlidingExpiration,
                    ExpireTimeSpan = tenant.CookieLifetime
                },
            };

            ClaimsIdentity.RoleClaimType = IdentityConstants.RoleClaimType;
            ClaimsIdentity.UserIdClaimType = IdentityConstants.UserIdClaimType;
            ClaimsIdentity.UserNameClaimType = IdentityConstants.UserNameClaimType;

            Password.RequiredLength = tenant.PasswordOptions.MinLength;
            Password.RequireDigit = tenant.PasswordOptions.RequireDigit;
            Password.RequireLowercase = tenant.PasswordOptions.RequireLower;
            Password.RequireNonAlphanumeric = tenant.PasswordOptions.RequireSpecial;
            Password.RequireUppercase = tenant.PasswordOptions.RequireUpper;
        }

    }
}
