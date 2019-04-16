using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security
{
    public class NullClaimValueResolver : ISecurityClaimValueResolver
    {
        public string Resolve()
        {
            return null;
        }
    }
}
