using System.Collections.Generic;

using IdentityServer4.Models;

namespace Angelo.Aegis.Configuration
{
    public class Clients
    {
        public static IEnumerable<IdentityServer4.Models.Client> Get()
        {
            return new List<IdentityServer4.Models.Client>
            {
               new ConnectClient()
            };
        }
    }
}