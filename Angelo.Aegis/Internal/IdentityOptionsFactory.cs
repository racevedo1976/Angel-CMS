using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;

using Angelo.Aegis.Configuration;
using Angelo.Common.Mvc.Saas;
using Angelo.Identity.Config;


namespace Angelo.Aegis.Internal
{
    public class IdentityOptionsFactory : OptionsFactory<IdentityOptions>
    {
        private IHttpContextAccessor _httpContextAccessor;

        public IdentityOptionsFactory(IServiceProvider provider, IHttpContextAccessor httpContextAccessor) : base(provider)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override IdentityOptions Create()
        {
            return new IdentityOptions();
        }

        public override void Configure(IdentityOptions options)
        {
            ConfigureCommonOptions(options);

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var tenant = _httpContextAccessor.HttpContext.GetTenant<AegisTenant>();
                if (tenant != null)
                {
                    ConfigureTenantOptions(options, tenant);
                }

            }
        }

        private void ConfigureCommonOptions(IdentityOptions options)
        {
            options.ClaimsIdentity.RoleClaimType = IdentityConstants.RoleClaimType;
            options.ClaimsIdentity.UserIdClaimType = IdentityConstants.UserIdClaimType;
            options.ClaimsIdentity.UserNameClaimType = IdentityConstants.UserNameClaimType;
        }

        private void ConfigureTenantOptions(IdentityOptions options, AegisTenant tenant)
        {
            var tenantKey = tenant.TenantKey;

            options.Cookies = new IdentityCookieOptions
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
                }
            };

            options.Password.RequiredLength = tenant.PasswordOptions.MinLength;
            options.Password.RequireDigit = tenant.PasswordOptions.RequireDigit;
            options.Password.RequireLowercase = tenant.PasswordOptions.RequireLower;
            options.Password.RequireNonAlphanumeric = tenant.PasswordOptions.RequireSpecial;
            options.Password.RequireUppercase = tenant.PasswordOptions.RequireUpper;
        }
    }
}
