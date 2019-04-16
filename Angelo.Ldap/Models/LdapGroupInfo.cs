using System.Collections.Generic;

namespace Angelo.Ldap
{
    public class LdapGroupInfo : LdapEntryInfo
    {
        public string GroupName { get; set; }
        public string ContainerName { get; set; }
        public List<string> Member { get; set; } // The list distinguished names of the entries that are members of this group.
        public List<string> MemberOf { get; set; } // The list distinguished names of the entries that this group is a member of.

        public LdapGroupInfo() : base()
        {
            Member = new List<string>();
            MemberOf = new List<string>();
        }
           
        public LdapGroupInfo(LdapGroupInfo entry) : base(entry)
        {
            GroupName = entry.GroupName;
            ContainerName = entry.ContainerName;
            Member = new List<string>();
            Member.AddRange(entry.Member);
            MemberOf = new List<string>();
            MemberOf.AddRange(entry.MemberOf);
        }

    }

   public class LdapGroupCollection : LdapEntryCollection<LdapGroupInfo> { }

}
