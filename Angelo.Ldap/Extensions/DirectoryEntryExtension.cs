using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Angelo.Ldap;

namespace System.DirectoryServices
{
    public static class DirectoryEntryExtension
    {
        const Int64 ACCOUNTDISABLE = 0x00000002; // The user account is disabled. 
        const Int64 LOCKOUT = 0x00000010;  // The account is currently locked out. 
        const Int64 PASSWORD_EXPIRED = 0x00800000; // The user password has expired. This flag is created by the system using data from the Pwd-Last-Set attribute and the domain policy. 

        public static string GetPropertyAsString(this DirectoryEntry entry, string name)
        {
            if (entry.Properties.Contains(name))
                return entry.Properties[name][0].ToString();
            else
                return string.Empty;
        }

        public static DateTime GetPropertyAsDateTime(this DirectoryEntry entry, string name)
        {
            DateTime dt = DateTime.MinValue;
            string dtStr = GetPropertyAsString(entry, name);
            if (!string.IsNullOrEmpty(dtStr))
                DateTime.TryParse(dtStr, out dt);
            return dt;
        }

        public static byte[] GetPropertyAsByteArray(this DirectoryEntry entry, string name)
        {
            byte[] byteArray = null;
            if (entry.Properties.Contains(name))
                byteArray = (byte[])entry.Properties[name][0];
            return byteArray;
        }

        public static string GetPropertyAsSID(this DirectoryEntry entry, string name)
        {
            if (entry.Properties.Contains(name))
            {
                byte[] byteArray = (byte[])entry.Properties[name][0];
                SecurityIdentifier si = new SecurityIdentifier(byteArray, 0);
                return si.ToString();
            }
            else
                return string.Empty;
        }

        public static Int64 GetPropertyAsInt64(this DirectoryEntry entry, string name)
        {
            if (entry.Properties.Contains(name))
            {
                Int64 value = 0;
                Int64.TryParse(entry.Properties[name][0].ToString(), out value);
                return value;
            }
            else
                return 0;
        }

        public static List<string> GetProperties(this DirectoryEntry entry, string name)
        {
            var list = new List<string>();
            foreach (var p in entry.Properties[name])
            {
                list.Add(p.ToString());
            }
            return list;
        }

        /// <summary>
        /// Returns the SID of the primary group assigned to the user.
        /// </summary>
        /// <note>
        /// The primaryGroupID is an interger number.  In order to find the SID of that group
        /// you take the SID of the assigned user and replace the last section of the user's SID
        /// with the primaryGroupID.
        /// For example: 
        ///     primaryGroupID = 1094
        ///     User SID = S-1-5-21-3623811015-3361044348-30300820-1013
        ///     Group SID = S-1-5-21-3623811015-3361044348-30300820-1094
        /// </note>
        private static string GetPrimaryGroupSID(DirectoryEntry entry, string userSID)
        {
            string primaryGroupID = GetPropertyAsString(entry, "primaryGroupID");
            int dashPos = userSID.LastIndexOf("-");
            if (string.IsNullOrEmpty(primaryGroupID) || (dashPos == -1))
                return string.Empty;
            else
                return userSID.Substring(0, dashPos + 1) + primaryGroupID;
        }

        public static LdapEntryInfo CopyTo(this DirectoryEntry entry, LdapEntryInfo info)
        {
            info.Guid = new Guid(entry.Guid.ToString()); // Break reference to entry.Guid so that the entry instance can go to garbage collection.
            info.Name = String.Copy(entry.Name);
            info.Path = String.Copy(entry.Path);
            info.ClassName = String.Copy(entry.SchemaClassName);
            info.DistinguishedName = GetPropertyAsString(entry, "distinguishedName");
            info.Created = GetPropertyAsDateTime(entry, "whenCreated");
            info.Modified = GetPropertyAsDateTime(entry, "whenChanged");
            info.SID = GetPropertyAsSID(entry, "objectSID");
            return info;
        }

        public static LdapUserInfo CopyTo(this DirectoryEntry entry, LdapUserInfo info)
        {
            CopyTo(entry, info as LdapEntryInfo);
            info.LoginName = GetPropertyAsString(entry, "sAMAccountName");
            info.UserPrincipalName = GetPropertyAsString(entry, "userPrincipalName");
            info.Email = GetPropertyAsString(entry, "mail");
            info.FirstName = GetPropertyAsString(entry, "givenName");
            info.MiddleName = GetPropertyAsString(entry, "initials");
            info.LastName = GetPropertyAsString(entry, "sn");
            info.PrimaryGroupSID = GetPrimaryGroupSID(entry, info.SID);
            Int64 userAccountControl = GetPropertyAsInt64(entry, "userAccountControl");
            info.LockedOut = (userAccountControl & LOCKOUT) > 0;
            info.Disabled = ((userAccountControl & ACCOUNTDISABLE) > 0) || ((userAccountControl & PASSWORD_EXPIRED) > 0);
            return info;
        }

        public static LdapGroupInfo CopyTo(this DirectoryEntry entry, LdapGroupInfo info)
        {
            CopyTo(entry, info as LdapEntryInfo);
            info.GroupName = GetPropertyAsString(entry, "sAMAccountName");
            info.ContainerName = GetPropertyAsString(entry, "cn");
            info.Member = GetProperties(entry, "member");
            info.MemberOf = GetProperties(entry, "memberOf");
            return info;
        }

        public static LdapFolderInfo CopyTo(this DirectoryEntry entry, LdapFolderInfo info)
        {
            CopyTo(entry, info as LdapEntryInfo);
            info.OU = GetPropertyAsString(entry, "ou");
            return info;
        }

    }

}

