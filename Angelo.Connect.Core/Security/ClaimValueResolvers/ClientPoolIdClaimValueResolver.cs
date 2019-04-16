using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class ClientPoolIdClaimValueResolver : ISecurityClaimValueResolver
    {
        private SiteContext _siteContext;

        public ClientPoolIdClaimValueResolver(SiteContext siteContext)
        {
            _siteContext = siteContext;

        }
        public string Resolve()
        {
            return _siteContext?.Client?.SecurityPoolId;
        }
    }
}
