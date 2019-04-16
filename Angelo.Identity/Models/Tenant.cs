using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class Tenant
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string OidcTitle { get; set; }
        public string OidcBanner { get; set; }
        public ICollection<TenantUri> OidcUris { get; set; }

        public ICollection<SecurityPool> SecurityPools { get; set; }

        public ICollection<Directory> Directories { get; set; }

        public ICollection<User> Users { get; set; }

    }
}


