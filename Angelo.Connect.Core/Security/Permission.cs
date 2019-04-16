using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class Permission
    {
        public Permission()
        {
            Permissions = new List<Permission>();
            Claims = new List<SecurityClaim>();
        }

        public string  Title { get; set; }

        public IList<SecurityClaim> Claims { get; set; }

        public IList<Permission> Permissions { get; set; }

        
    }
}
