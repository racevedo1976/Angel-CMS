using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class SiteIdClaimValueResolver : ISecurityClaimValueResolver
    {
        private AdminContext _adminContext;

        public string Resolve()
        {
            return _adminContext?.SiteId;
        }

        public SiteIdClaimValueResolver(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

    }
}
