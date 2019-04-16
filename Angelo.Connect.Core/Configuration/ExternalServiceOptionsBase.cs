using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Configuration
{
    public abstract class ExternalServiceOptionsBase
    {
        public string Authority { get; set; }
        public EndPointOptions EndPoints { get; set; }

        public class EndPointOptions
        {
            public string Authorize { get; set; }
            public string UserInfo { get; set; }
            public string ObtainToken { get; set; }
            public string RevokeToken { get; set; }
            public string ValidateToken { get; set; }
            public string Logout { get; set; }
            public string Meta { get; set; }

            public string TenantUsers { get; set; }
            public string TenantRoles { get; set; }

            public string PoolUsers { get; set; }
            public string PoolRoles { get; set; }

            public string Ldap { get; set; }
            public string SetPassword { get; set; }
        }
    }
}
