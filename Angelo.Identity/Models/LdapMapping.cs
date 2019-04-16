using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class LdapMapping
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string ObjectGuid { get; set; }
        public string RoleId { get; set; }
        
        public Role Role { get; set; }

    }
}
