using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Configuration
{
    public class AegisEmptyTenant : AegisTenant
    {
        public AegisEmptyTenant()
        {
            TenantId = null;
            TenantKey = null;
            SiteTitle = "Aegis Identity Server";
            SiteBanner = "/img/default-banner.png";
            AuthSchemeInternal = "aegis.internal";
            AuthSchemeExternal = "aegis.external";
            Clients = new List<IdentityServer4.Models.Client>();
            PasswordOptions = new PasswordOptions
            {
                MinLength = 6,
                RequireDigit = true,
                RequireLower = true,
                RequireSpecial = false,
                RequireUpper = true
            };
            ProviderOptions = new ExternalProviders();

        }
    }
}
