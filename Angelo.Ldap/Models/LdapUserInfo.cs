using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    public class LdapUserInfo : LdapEntryInfo
    {
        public string LoginName { get; set; }
        public string UserPrincipalName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string PrimaryGroupSID { get; set; }
        public bool LockedOut { get; set; }
        public bool Disabled { get; set; }
        public LdapGroupCollection Groups { get; set; }

        public LdapUserInfo() : base()
        {
            Groups = new LdapGroupCollection();
        }

        public LdapUserInfo(LdapUserInfo entry) : base(entry)
        {
            LoginName = entry.LoginName;
            UserPrincipalName = entry.UserPrincipalName;
            Email = entry.Email;
            FirstName = entry.FirstName;
            MiddleName = entry.MiddleName;
            LastName = entry.LastName;
            PrimaryGroupSID = entry.PrimaryGroupSID;
            LockedOut = entry.LockedOut;
            Disabled = entry.Disabled;
            Groups = new LdapGroupCollection();
            foreach (LdapGroupInfo group in entry.Groups)
                Groups.Add(new LdapGroupInfo(group));
        }

    }

    class LdapUserCollection : LdapEntryCollection<LdapUserInfo> { }

}
