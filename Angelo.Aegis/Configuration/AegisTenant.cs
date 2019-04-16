using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Configuration;
using IdentityServer4.Models;

namespace Angelo.Aegis.Configuration
{
    public class AegisTenant
    {
        public string TenantId { get; set; }
        public string TenantKey { get; set; }

        public string SiteTitle { get; set; }

        public string SiteBanner { get; set; }
        
        public string AuthSchemeInternal { get; set; }

        public string AuthSchemeExternal { get; set; }

        public bool CookieSlidingExpiration { get; set; } = false;

        public TimeSpan CookieLifetime { get; set; } = TimeSpan.FromDays(5);

        public List<Client> Clients { get; set; }

        public PasswordOptions PasswordOptions { get; set; }

        public ExternalProviders ProviderOptions { get; set;}
    }
}
