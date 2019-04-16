using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class ClientIdClaimValueResolver : ISecurityClaimValueResolver
    {
        private AdminContext _adminContext;

        public ClientIdClaimValueResolver(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public string Resolve()
        {
            return _adminContext?.ClientId;
        }
    }
}
