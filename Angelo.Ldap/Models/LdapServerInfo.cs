using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    public class LdapServerInfo
    {
        public string Path { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public LdapServerInfo()
        {
        }

        public LdapServerInfo(LdapServerInfo info)
        {
            Path = info.Path;
            Username = info.Username;
            Password = info.Password;
        }

    }
}
