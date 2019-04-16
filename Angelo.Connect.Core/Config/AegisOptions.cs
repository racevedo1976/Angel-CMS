using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
//using Microsoft.IdentityModel.Tokens;
using Angelo.Connect.Configuration;

namespace Angelo.Connect
{
    public class AegisOptions : ExternalServiceOptionsBase
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdClaimType { get; set; }
        public string NameClaimType { get; set; }
        public string RoleClaimType { get; set; }

        public List<string> Scopes { get; set; }
    }
}
