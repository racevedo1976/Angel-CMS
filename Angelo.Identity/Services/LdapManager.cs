using Angelo.Identity.Models;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Services
{
    public class LdapManager
    {

        private static LdapConnection _conn;

        public LdapManager()
        {

        }

        public LdapConnection GetConnection()
        {
            LdapConnection ldapConn = _conn as LdapConnection;
            int port = 389; 

            if (ldapConn == null)
            {
                // Creating an LdapConnection instance 
                ldapConn = new LdapConnection() { SecureSocketLayer = false };

                //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                ldapConn.Connect("10.0.1.81", port);

                //Bind function with null user dn and password value will perform anonymous bind to LDAP server 
                ldapConn.Bind(@"mcpss2\ldap", "n0b0dykn0w$");
            }

            return ldapConn;
        }

        public HashSet<string> SearchForGroup(string groupName)
        {
            var ldapConn = GetConnection();
            var groups = new HashSet<string>();

            var searchBase = "dc=mcpss2,dc=com";
            var filter = $"(&(objectClass=organizationalUnit)(ou={groupName}))";
            var scope = LdapConnection.SCOPE_ONE;
            var search = ldapConn.Search(searchBase, scope, filter, null, false);
            while (search.hasMore())
            {
                try
                {
                    var nextEntry = search.next();
                    groups.Add(nextEntry.DN);
                    //var childGroups = GetChildren(string.Empty, nextEntry.DN);
                    //foreach (var child in childGroups)
                    //{
                    //    groups.Add(child);
                    //}
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                    throw;
                }
              
            }

            return groups;
        }

        private HashSet<string> GetChildren(string searchBase, string groupDn, string objectClass = "group")
        {
            var ldapConn = GetConnection();
            var listNames = new HashSet<string>();

            var filter = $"(&(objectClass={objectClass})(memberOf={groupDn}))";
            
            var search = ldapConn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

            while (search.hasMore())
            {
                try
                {
                    
                    var nextEntry = search.next();
                    listNames.Add(nextEntry.DN);
                    var children = GetChildren(string.Empty, nextEntry.DN);
                    foreach (var child in children)
                    {
                        listNames.Add(child);
                    }
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                   
                }
               
            }

            return listNames;
        }

        public void SearchForUser(string company, HashSet<string> groups = null)
        {
            var ldapConn = GetConnection();
            var users = new HashSet<string>();

            string groupFilter = (groups?.Count ?? 0) > 0 ?
                $"(|{string.Join("", groups.Select(x => $"(memberOf={x})").ToList())})" :
                string.Empty;
            var searchBase = string.Empty;
            string filter = $"(&(objectClass=user)(objectCategory=person)(company={company}){groupFilter})";
            var search = ldapConn.Search(searchBase, LdapConnection.SCOPE_SUB, filter, null, false);

            while (search.hasMore())
            {
                var nextEntry = search.next();
                nextEntry.getAttributeSet();
                users.Add(nextEntry.DN);
            }
        }

        public ICollection<LdapNodeObject> GetDirectoryEntries(LdapDomain ldapDomain, string searchBase)
        {
            IList<LdapNodeObject> entries = new List<LdapNodeObject>();

            var ldapConn = GetConnection(ldapDomain);

            var filter = $"(objectClass=*)";
            var scope = LdapConnection.SCOPE_ONE;
            var search = ldapConn.Search(searchBase, scope, filter, null, false);
            while (search.hasMore())
            {
                try
                {
                    var nextEntry = search.next();
                    var nodeObject = new LdapNodeObject
                    {
                        DistinguishedName = nextEntry.DN,
                        Id = nextEntry.DN,
                        ObjectGuid = GetEntryAttribute(nextEntry, "objectGuid"),
                        OU = GetEntryAttribute(nextEntry, "ou"),
                        Name = GetEntryAttribute(nextEntry, "name"),
                        HasChildren = true    //TODO figure out if node has children.
                    };
                    entries.Add(nodeObject);
                   
                }
                catch (LdapException e)
                {
                    Console.WriteLine("Error: " + e.LdapErrorMessage);
                    //Exception is thrown, go for next entry
                    continue;
                    
                }

            }

            return entries;
        }

        private string GetEntryAttribute(LdapEntry ldapEntry, string attributeName)
        {
            try
            {
                if (attributeName.ToLower() == "objectguid")
                {
                    var attValue = ldapEntry.getAttribute(attributeName);
                    if (attValue != null)
                        return new Guid((Byte[])(Array)attValue.ByteValue).ToString();
                    else
                        return "";
                }

                else
                    return ldapEntry.getAttribute(attributeName)?.StringValue ?? "";
            }
            catch (Exception)
            {

                //we are just eating this error on a specific attribute, Not a real reason why to stopped the process
                // unless its is required value that must be use somewhere else, but so far we are just displaying info only.
                return "";
                //throw;
            }

        }



        public IList<KeyValuePair<string, string>> GetEntryAttributes(LdapDomain ldapDomain, string searchDn)
        {

            List<KeyValuePair<string, string>> _attributes = new List<KeyValuePair<string, string>>();

            IList<LdapNodeObject> entries = new List<LdapNodeObject>();

            var ldapConn = GetConnection(ldapDomain);

            var entry = ldapConn.Read(searchDn);

            // Get the attribute set of the entry
            LdapAttributeSet attributeSet = entry.getAttributeSet();
            System.Collections.IEnumerator ienum = attributeSet.GetEnumerator();

            // Parse through the attribute set to get the attributes and
            //the corresponding values
            while (ienum.MoveNext())
            {
                LdapAttribute attribute = (LdapAttribute)ienum.Current;
                string attributeName = attribute.Name;
                string attributeVal = attribute.StringValue;
                if (attributeName == "objectGUID")
                        attributeVal = new Guid((Byte[])(Array)attribute?.ByteValue).ToString();
                
                _attributes.Add(new KeyValuePair<string, string>(attributeName, attributeVal));
                
            }

            return _attributes;
        }

        public LdapConnection GetConnection(LdapDomain ldapDomain)
        {
            LdapConnection ldapConn = _conn as LdapConnection;

            int port;
            if (!ldapDomain.UseSsl)
                port = LdapConnection.DEFAULT_PORT;
            else
                port = LdapConnection.DEFAULT_SSL_PORT;
             

            if (ldapConn == null)
            {
                try
                {
                    ldapConn = new LdapConnection() { SecureSocketLayer = false };

                    //Connect function will create a socket connection to the server - Port 389 for insecure and 3269 for secure    
                    ldapConn.Connect(ldapDomain.Host, port);

                    //Bind function with null user dn and password value will perform anonymous bind to LDAP server
                    //First figure the user structure 
                    string lpdaUser = "";
                    if (ldapDomain.User.Contains(@"\") || (ldapDomain.User.Contains("\\")))
                    {
                        lpdaUser = ldapDomain.User;
                    }
                    else
                    {
                        lpdaUser = $@"{ldapDomain.Domain}\{ldapDomain.User}";
                    }

                    ldapConn.Bind(lpdaUser, ldapDomain.Password);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
              
               
            }

            return ldapConn;
        }
    }
}
