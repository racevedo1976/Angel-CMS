using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Configuration
{
    public class ServerOptions
    {
        public string ApplicationName { get; set; }
        public DataOptions Data { get; set; }
        public OpenIdOptions OpenId { get; set;  }

        public class DataOptions
        {
            public string ConnectionString { get; set; }
        }

        public class OpenIdOptions
        {
            public bool RequireSsl { get; set; }
            public string CertificateName { get; set; }
            public string CookieSchemeInternal { get; set; }
            public string CookieSchemeExternal { get; set; }
            public PasswordOptions Password { get; set; }
            public ExternalProviders Providers { get; set; }
        }
    }
}
