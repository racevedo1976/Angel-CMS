using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    public class LdapUserQuery
    {
        public LdapServerInfo Server { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }

        public LdapUserQuery()
        {
            Server = new LdapServerInfo();
        }
    }
}
