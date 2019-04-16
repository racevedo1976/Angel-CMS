using System.Collections.Generic;

namespace Angelo.Ldap
{
    public class LdapFolderInfo : LdapEntryInfo
    {
        /// <summary>
        /// The Organization Unit (can also be used as the display name of the folder).
        /// </summary>
        public string OU { get; set; }

        public LdapFolderInfo() : base()
        {
        }

        public LdapFolderInfo(LdapFolderInfo entry) : base(entry)
        {
            OU = entry.OU;
        }

    }

    public class LdapFolderCollection : LdapEntryCollection<LdapFolderInfo> { }

}
