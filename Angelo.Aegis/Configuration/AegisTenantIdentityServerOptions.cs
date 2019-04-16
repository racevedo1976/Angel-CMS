using System;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Http;

namespace Angelo.Aegis.Configuration
{
    public class AegisTenantIdentityServerOptions : IdentityServerOptions
    {
        public AegisTenantIdentityServerOptions(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            // just to be sure, we are in a tenant context
            var tenantContext = httpContextAccessor.HttpContext.GetTenantContext<AegisTenant>();

            if (tenantContext == null)
                throw new ArgumentNullException(nameof(tenantContext));

            // **** now we can have tenantspecific IdentityServerOptions           
            Authentication.AuthenticationScheme = tenantContext.Tenant.AuthSchemeInternal;
            Authentication.CookieSlidingExpiration = tenantContext.Tenant.CookieSlidingExpiration;
            Authentication.CookieLifetime = tenantContext.Tenant.CookieLifetime;
        }
    }
}
