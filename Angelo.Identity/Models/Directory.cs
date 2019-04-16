using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class Directory
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public ICollection<User> Users { get; set; }

        public ICollection<DirectoryMap> DirectoryMap { get; set; }

        public Tenant Tenant { get; set; }

        public LdapDomain LdapDomain { get; set; }
    }
}
