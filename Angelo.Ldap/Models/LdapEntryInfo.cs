using System;
using System.Collections.Generic;
using System.Linq;

namespace Angelo.Ldap
{
    public class LdapEntryInfo
    {
        public Guid Guid { get; set; }
        public string SID { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string ClassName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        public LdapEntryInfo()
        {
            Guid = new Guid();
        }

        public LdapEntryInfo(LdapEntryInfo entry)
        {
            Guid = new Guid(entry.Guid.ToString()); // Break reference to entry.Guid so that the entry instance can go to garbage collection.
            Name = entry.Name;
            Path = entry.Path;
            ClassName = entry.ClassName;
            DistinguishedName = entry.DistinguishedName;
            Created = entry.Created;
            Modified = entry.Modified;
            SID = entry.SID;
        }

    }


    public class LdapEntryCollection<TEntry> : List<TEntry> where TEntry: LdapEntryInfo
    {
        /// <summary>
        /// Retruns the index of the entry Info havining the specified distinctiveName.
        /// Returns -1 if the item is not found.
        /// </summary>
        public int IndexOfDN(string distinguishedName)
        {
            TEntry entry;
            int index1 = -1;
            int index2 = Count;
            while ((index2 > 0) && (index1 == -1))
            {
                index2 = index2 - 1;
                entry = this[index2];
                if (entry.DistinguishedName.Equals(distinguishedName, StringComparison.OrdinalIgnoreCase))
                    index1 = index2;
            }
            return index1;
        }

        /// <summary>
        /// Retruns the index of the entry Info havining the specified Guid.
        /// Returns -1 if the item is not found.
        /// </summary>
        public int IndexOfGuid(string guid)
        {
            Guid guidData;
            if (!Guid.TryParse(guid, out guidData))
                guidData = Guid.Empty;
            return IndexOfGuid(guidData);
        }

        /// <summary>
        /// Retruns the index of the entry Info havining the specified Guid.
        /// Returns -1 if the item is not found.
        /// </summary>
        public int IndexOfGuid(Guid guid)
        {
            TEntry entry;
            int index1 = -1;
            int index2 = Count;
            while ((index2 > 0) && (index1 == -1))
            {
                index2 = index2 - 1;
                entry = this[index2];
                if (entry.Guid.Equals(guid))
                    index1 = index2;
            }
            return index1;
        }

    }


}
