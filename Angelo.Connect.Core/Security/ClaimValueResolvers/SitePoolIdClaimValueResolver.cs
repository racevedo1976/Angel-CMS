using System;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Security
{
    public class SitePoolIdClaimValueResolver : ISecurityClaimValueResolver
    {
        private SiteContext _siteContext;

        public SitePoolIdClaimValueResolver(SiteContext siteContext)
        {
            _siteContext = siteContext;
        }

        public string Resolve()
        {
            return _siteContext?.SecurityPoolId;
        }
    }
}