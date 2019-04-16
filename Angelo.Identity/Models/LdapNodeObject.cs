using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class LdapNodeObject
    {
        public string Id { get; set; }

        public string DistinguishedName { get; set; }
        public string Name { get; set; }

        public string ObjectGuid { get; set; }

        public string OU { get; set; }
        public bool HasChildren { get; set; }
    }
}
