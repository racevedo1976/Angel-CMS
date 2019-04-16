using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class TenantUri
    {
        public string Id { get; set; }
        public TenantUriType Type { get; set; }
        public string Uri { get; set; }
        public string TenantId { get; set; }
        public Tenant Tenant { get; set; }
    }

    public enum TenantUriType
    {
        OidcSignin = 10,
        OidcPostLogout = 12
    }
}
