using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class LdapDomain
    {
        public LdapDomain()
        {
            //Id = Guid.NewGuid().ToString("N");
            UseSsl = false;
        }

        public string Id { get; set; }       
        public string Host { get; set; }
        public string Domain { get; set; }

        public string User { get; set; }
        public string Password { get; set; }
       
        public string LdapBaseDn { get; set; }

        public bool UseSsl { get; set; }

        public string DirectoryId { get; set; }
        public Directory Directory { get; set; }
    }
}
