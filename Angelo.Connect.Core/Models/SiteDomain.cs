using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class SiteDomain
    {
        public string SiteId { get; set; }
        public string DomainKey { get; set; }
        public bool IsDefault { get; set; }
        public Site Site { get; set; }

        public string ToSignInRedirectUri()
        {
            return DomainKey + "/signin-oidc";
        }

        public string ToPostLogoutRedirectUri()
        {

            return DomainKey + "/user/loggedout";
        }
    }
}
