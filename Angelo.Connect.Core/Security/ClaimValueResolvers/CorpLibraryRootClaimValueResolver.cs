using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class CorpLibraryRootClaimValueResolver : ISecurityClaimValueResolver
    {
        public string Resolve()
        {
            return "/corp/shared/";
        }
    }
}
