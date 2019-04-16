using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    /// <summary>
    /// Use this class to parse the LDAP Distinguished Name property.
    /// DNs are composed of multiple parts and commonly used to locate entries.
    /// For example: CN=Test_Group,OU=MPCS_Groups,DC=mcpss2,DC=com
    /// </summary>
    public class LdapDnParts : List<LdapDnParts.Part>
    {
        public class Part
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public static LdapDnParts Parse(string distinguishedName)
        {
            return Parse(distinguishedName, "");
        }

        /// <summary>
        /// Creates a list of the parts contained in the specified distringuishedName.
        /// Use the partType to specifiy with parts you wish to include.
        /// For example: Use "cn" to only include common name parts in the list.
        /// Note: leave the partType empty to include all the parts in the list.
        /// </summary>
        /// <param name="distinguishedName"></param>
        /// <param name="partType"></param>
        /// <returns></returns>
        public static LdapDnParts Parse(string distinguishedName, string partType)
        {
            int equalPos;
            LdapDnParts dn = new LdapDnParts();
            string[] parts = distinguishedName.Split(',');
            foreach (string item in parts)
            {
                equalPos = distinguishedName.IndexOf('=');
                if (equalPos > -1)
                {
                    Part newPart = new Part();
                    newPart.Name = item.Substring(0, equalPos - 1).Trim();
                    newPart.Value = item.Substring(equalPos + 1);
                    if ((partType == "") || (partType.Equals(newPart.Name, StringComparison.OrdinalIgnoreCase)))
                        dn.Add(newPart);
                }
            }
            return dn;
        }
    }
}





