using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angelo.Ldap
{
    /// <summary>
    /// Used to simplify the Directory Service API.
    /// </summary>
    class LdapHelper
    {
        private LdapPathBuilder _root;

        /// <summary>
        /// The root directory to perform searches on.  
        /// Ex: LDAP://10.0.1.81/DC=mcpss2,DC=com
        /// </summary>
        public string RootPath
        {
            get { return _root.Path; }
            set { _root.Path = value; }
        }

        /// <summary>
        /// Read-only value of the host contained in the RootPath.
        /// </summary>
        public string Host
        {
            get { return _root.Host; }
        }

        /// <summary>
        /// The login name of the account having access to perform searches on the LDAP server.
        /// </summary>
        private string _username;
        public string Username
        {
            get { return _username; }
            set { _username = value; }
        }

        /// <summary>
        /// The login name of the account having access to perform searches on the LDAP server.
        /// </summary>
        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// The authentication method to use to connect to the server.
        /// </summary>
        private AuthenticationTypes _authenticationType;
        public AuthenticationTypes AuthenticationType
        {
            get { return _authenticationType; }
            set { _authenticationType = value; }
        }

        public LdapHelper()
        {
            _root = new LdapPathBuilder();
            AuthenticationType = AuthenticationTypes.Secure;
        }

        /// <param name="serverInfo">The LDAP server and root path that should be used to perform searches in.</param>
        public LdapHelper(LdapServerInfo serverInfo) : this()
        {
            _root.Path = serverInfo.Path;
            Username = serverInfo.Username;
            Password = serverInfo.Password;
        }

        /// Returns the base or root LDAP entry that can be used to serach.
        protected DirectoryEntry GetRootEntry()
        {
            return new DirectoryEntry(_root.Path, _username, _password, _authenticationType);
        }

        ///Returns the LDAP entry having the specified distinguished name or null if the entry is not found.
        protected DirectoryEntry GetEntryByDN(string distinguishedName)
        {
            string path = _root.ComposePath(distinguishedName);
            DirectoryEntry entry = new DirectoryEntry(path, _username, _password, _authenticationType);
            return entry;
        }

        ///Returns the LDAP entry having the specified objectGuid or null if the entry is not found.
        protected DirectoryEntry GetEntryByGuid(Guid guid)
        {
            string path = _root.ComposePath("<GUID=" + guid.ToString() + ">");
            DirectoryEntry entry = new DirectoryEntry(path, _username, _password, _authenticationType);
            return entry;
        }

        /// <param name="entry">The root LDAP entry to search.</param>
        /// <param name="filter">LDAP query string (ex: "(&(objectClass=user)(sAMAccountName=bubba))".</param>
        /// <param name="searchSubtrees">Set to false to only search the current entry level.
        /// <returns>Returns the first item having the specified filter query.</returns>
        protected SearchResult GetSearchResult(DirectoryEntry entry, string filter, bool searchSubtrees)
        {
            SearchResult result = null;
            try
            {
                DirectorySearcher searcher = new DirectorySearcher(entry);
                if (searchSubtrees)
                    searcher.SearchScope = SearchScope.Subtree;
                else
                    searcher.SearchScope = SearchScope.OneLevel;
                searcher.Filter = filter;
                result = searcher.FindOne();
            }
            catch
            {
            }
            return result;
        }

        /// <param name="entry">The root LDAP entry to search.</param>
        /// <param name="filter">LDAP query string (ex: "(objectClass=group)".</param>
        /// <param name="searchSubtrees">Set to false to only search the current entry level.
        /// <returns>Returns the list of items having the specified filter query.</returns>
        protected SearchResultCollection GetSearchResults(DirectoryEntry entry, string filter, bool searchSubtrees)
        {
            DirectorySearcher searcher = new DirectorySearcher(entry);
            if (searchSubtrees)
                searcher.SearchScope = SearchScope.Subtree;
            else
                searcher.SearchScope = SearchScope.OneLevel;
            searcher.Filter = filter;
            SearchResultCollection result = searcher.FindAll();
            return result;
        }

        protected string ConvertGuidToOctetString(Guid guid)
        {
            byte[] byteGuid = guid.ToByteArray();
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteGuid)
            {
                sb.Append(@"\");
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        ///Returns true if a connection to the server was sucessfully made.
        /// </summary>
        /// <param name="errMsg">If a connection cannot be made, then the error message will be stored in this output parameter.</param>
        public bool TestConnection(out string errMsg)
        {
            errMsg = String.Empty;
            bool isValid = false;
            try
            {
                object nativeObject = GetRootEntry().NativeObject; // Force authentication by requesing the native COM object.
                isValid = true;
            }
            catch (Exception e)
            {
                errMsg = e.Message;
            }
            return isValid;
        }

        /// <summary>
        /// Use this method to get the distinguished name (DN) of the root direcotry when only the host is spcified in the RootPath.
        /// ex: LDAP://domain.com/
        /// </summary>
        /// <returns>Returns the Distinguished Name of the root directory.</returns>
        public string GetBaseDN()
        {
            var entry = GetRootEntry().CopyTo(new LdapEntryInfo());
            return entry.DistinguishedName;
        }

        /// <summary>
        /// Use this method to authenticate a specified user with the LDAP server.
        /// </summary>
        /// <param name="user">The user account to authenticate.</param>
        /// <param name="password">The password of the user to authenticate.</param>
        /// <returns>Returns true if the specified user credentials successfully validate.</returns>
        public bool AuthenticateUser(LdapUserInfo user, string password)
        {
            return AuthenticateUser(user.LoginName, password);
        }

        /// <summary>
        /// Use this method to authenticate a specified user with the LDAP server.
        /// </summary>
        /// <param name="username">The login name of the user account to authenticate.</param>
        /// <param name="password">The password of the user to authenticate.</param>
        /// <returns>Returns true if the specified user credentials successfully validate.</returns>
        public bool AuthenticateUser(string username, string password)
        {
            bool authentic = false;
            try
            {
                DirectoryEntry entry = new DirectoryEntry(_root.Path, username, password, _authenticationType);
                object nativeObject = entry.NativeObject;
                authentic = true;
            }
            catch (Exception)
            {
            }
            return authentic;
        }

        /// <summary>
        /// Retrieves the specified user information from the LDAP server.
        /// </summary>
        /// <param name="samAccountName">The login name of the user account to be retrieved.</param>
        /// <param name="loadGroups">Set to ture to load the Groups collection with those groups assigned to the user.</param>
        public LdapUserInfo GetUserByLogin(string samAccountName, bool loadGroups = true)
        {
            string filter = $@"(&(objectCategory=person)(objectClass=user)(sAMAccountName={samAccountName}))";
            SearchResult res = GetSearchResult(GetRootEntry(), filter, searchSubtrees: true);
            if (res == null)
            {
                return null;
            }
            else
            {
                var entry = res.GetDirectoryEntry();
                var user = entry.CopyTo(new LdapUserInfo());
                if (loadGroups) LoadGroupsAssignedToUser(user);
                return user;
            }
        }

        /// <summary>
        /// Retrieves the specified user information from the LDAP server or null if user is not found.
        /// </summary>
        /// <param name="guid">The objectGUID of the user account to be retrieved.</param>
        /// <param name="loadGroups">Set to ture to load the Groups collection with those groups assigned to the user.</param>
        public LdapUserInfo GetUserByGuid(Guid guid, bool loadGroups = true)
        {
            DirectoryEntry entry = GetEntryByGuid(guid);
            if (entry == null)
                return null;
            else
            {
                LdapUserInfo user = entry.CopyTo(new LdapUserInfo());
                if (loadGroups) LoadGroupsAssignedToUser(user);
                return user;
            }
        }

        /// <summary>
        /// Retrieves the specified user information from the LDAP server or null if user is not found.
        /// </summary>
        /// <param name="sid">The SID string of the user account to be retrieved.</param>
        /// <param name="loadGroups">Set to ture to load the Groups collection with those groups assigned to the user.</param>
        public LdapUserInfo GetUserBySID(string sid, bool loadGroups = true)
        {
            string filter = $@"(&(objectCategory=person)(objectClass=user)(objectSID={sid}))";
            SearchResult res = GetSearchResult(GetRootEntry(), filter, searchSubtrees: true);
            if (res == null)
            {
                return null;
            }
            else
            {
                var entry = res.GetDirectoryEntry();
                var user = entry.CopyTo(new LdapUserInfo());
                if (loadGroups) LoadGroupsAssignedToUser(user);
                return user;
            }
        }

        /// <summary>
        /// Retrieves the specified group information from the LDAP server or null if group is not found.
        /// </summary>
        /// <param name="GroupName">The sAMAccountName of the group to be retrieved.</param>
        public LdapGroupInfo GetGroupByName(string GroupName)
        {
            string filter = $@"(&(objectClass=group)(sAMAccountName={GroupName}))";
            SearchResult res = GetSearchResult(GetRootEntry(), filter, searchSubtrees: true);
            if (res == null)
            {
                return null;
            }
            else
            {
                var entry = res.GetDirectoryEntry();
                var group = entry.CopyTo(new LdapGroupInfo());
                return group;
            }
        }

        /// <summary>
        /// Retrieves the specified group information from the LDAP server or null if group is not found.
        /// </summary>
        /// <param name="guid">The GUID of the the group to be retrieved.</param>
        public LdapGroupInfo GetGroupByGuid(Guid guid)
        {
            DirectoryEntry entry = GetEntryByGuid(guid);
            if (entry == null)
                return null;
            else
                return entry.CopyTo(new LdapGroupInfo());
        }

        /// <summary>
        /// Retrieves the specified groups from the LDAP server.
        /// </summary>
        /// <param name="guid">The GUID of the the group to be retrieved.</param>
        public LdapGroupCollection GetGroupsByGuids(List<Guid> guids)
        {
            var groups = new LdapGroupCollection();
            if (guids.Count == 0) return groups;
            var sb = new StringBuilder();
            sb.Append("(&(objectClass=group)");
            if (guids.Count > 1) sb.Append("(|");
            foreach (var guid in guids)
                sb.Append($@"(objectGUID={ConvertGuidToOctetString(guid)})");
            if (guids.Count > 1) sb.Append(")");
            sb.Append(")");
            string filter = sb.ToString();
            SearchResultCollection resList = GetSearchResults(GetRootEntry(), filter, searchSubtrees: true);
            foreach (SearchResult res in resList)
            {
                var entry = res.GetDirectoryEntry();
                var group = entry.CopyTo(new LdapGroupInfo());
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// Retrieves the specified group information from the LDAP server or null if group is not found.
        /// </summary>
        /// <param name="sid">The SID string of the the group to be retrieved.</param>
        /// <returns></returns>
        public LdapGroupInfo GetGroupBySID(string sid)
        {
            string filter = $@"(&(objectClass=group)(objectSID={sid}))";
            SearchResult res = GetSearchResult(GetRootEntry(), filter, searchSubtrees: true);
            if (res == null)
            {
                return null;
            }
            else
            {
                var entry = res.GetDirectoryEntry();
                var group = entry.CopyTo(new LdapGroupInfo());
                return group;
            }
        }

        /// <summary>
        /// Retrieves the specified group information from the LDAP server or null if group is not found.
        /// </summary>
        /// <param name="distinguishedName">The distinguishedName of the the group to be retrieved.</param>
        /// <returns></returns>
        public LdapGroupInfo GetGroupByDN(string distinguishedName)
        {
            DirectoryEntry entry = GetEntryByDN(distinguishedName);
            if (entry == null)
            {
                return null;
            }
            else
            {
                var group = entry.CopyTo(new LdapGroupInfo());
                return group;
            }
        }

        /// <summary>
        /// Retrieves a list of user accounts that are assigned to the specified group and/or the children of that group.
        /// note: The the Groups list for each found user will not be populated.  You will need to call the LoadGroupsAssignedToUser method to populate this list.
        /// </summary>
        public LdapUserCollection GetUsersInGroup(LdapGroupInfo group)
        {
            var groups = new LdapGroupCollection();
            groups.Add(group);
            return GetUsersInGroups(groups);
        }

        /// <summary>
        /// Retrieves a list of user accounts that are assigned to the specified list of groups and/or the children of those groups.
        /// note: The the Groups list for each found user will not be populated.  You will need to call the LoadGroupsAssignedToUser method to populate this list.
        /// </summary>
        public LdapUserCollection GetUsersInGroups(LdapGroupCollection groups)
        {
            var users = new LdapUserCollection();
            if (groups.Count == 0) return users;

            var childGroups = new LdapGroupCollection();
            childGroups.AddRange(groups);
            foreach (var childGroup in groups)
                LoadChildrenOfGroup(childGroup, childGroups);

            var sb = new StringBuilder();
            var dnList = new List<string>();
            var groupIdList = new List<string>();
            foreach (var group in childGroups)
            {
                if (!dnList.Contains(group.DistinguishedName))
                {
                    dnList.Add(group.DistinguishedName);
                    sb.Append($@"(memberOf={group.DistinguishedName})");
                }
                int dashPos = group.SID.LastIndexOf("-");
                if (dashPos > -1)
                {
                    string primaryGroupID = group.SID.Substring(dashPos + 1);
                    if (!groupIdList.Contains(primaryGroupID))
                    {
                        groupIdList.Add(primaryGroupID);
                        sb.Append($@"(primaryGroupID={primaryGroupID})");
                    }
                }
            }
            if ((dnList.Count + groupIdList.Count) > 1)
            {
                sb.Insert(0, "(&(objectCategory=person)(objectClass=user)(|");
                sb.Append("))");
            }
            else
            {
                sb.Insert(0, "(&(objectCategory=person)(objectClass=user)");
                sb.Append(")");
            }
            string filter = sb.ToString();
            SearchResultCollection resList = GetSearchResults(GetRootEntry(), filter, searchSubtrees: true);
            foreach (SearchResult userRes in resList)
            {
                var entry = userRes.GetDirectoryEntry();
                var user = entry.CopyTo(new LdapUserInfo());
                if (users.IndexOfDN(user.DistinguishedName) == -1)
                    users.Add(user);
            }
            return users;
        }

        /// <summary>
        /// Populates the user.Groups collection with those groups that are assigned to the specified user.
        /// note: Any groups in the user.Groups collection before calling this method will be removed.
        /// </summary>
        /// <note>
        /// A user will also belong to the parents of the group that he/she is assigned to.
        /// So, also return the parents of assigned groups.
        /// </note>
        public int LoadGroupsAssignedToUser(LdapUserInfo user)
        {
            user.Groups.Clear();
            string filter = $@"(&(objectCategory=group)(member={user.DistinguishedName}))";
            SearchResultCollection resList = GetSearchResults(GetRootEntry(), filter, searchSubtrees: true);
            foreach (SearchResult groupRes in resList)
            {
                var entry = groupRes.GetDirectoryEntry();
                var group = entry.CopyTo(new LdapGroupInfo());
                user.Groups.Add(group);
            }

            // Note: AD does not include the primary group in the member list.
            var primaryGroup = GetGroupBySID(user.PrimaryGroupSID);
            if (primaryGroup != null)
                user.Groups.Add(primaryGroup);

            // Load the parent groups.
            var localGroups = user.Groups.ToArray();
            foreach (var group in localGroups)
                LoadParentsOfGroup(group, user.Groups);

            return user.Groups.Count; 
        }

        //public Task<LdapGroupCollection> GetGroupsAssignedToUserAsync(LdapUserInfo user)
        //{
        //    return Task<LdapGroupCollection>.Run(() => GetGroupsAssignedToUser(user));
        //}

        /// <summary>
        /// Recursive method to retrieve the parent groups of the spcified group.
        /// </summary>
        /// <param name="group">The child group of the parent groups to be found.</param>
        /// <param name="groups">A collection of all of the groups that have already been added to the list.</param>
        protected void LoadParentsOfGroup(LdapGroupInfo group, LdapGroupCollection groups)
        {
            if (group.MemberOf.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var parentDN in group.MemberOf)
                    sb.Append($@"(distinguishedName={parentDN})");
                if (group.MemberOf.Count > 1)
                {
                    sb.Insert(0, "(&(objectClass=group)(|");
                    sb.Append("))");
                }
                else
                {
                    sb.Insert(0, "(&(objectClass=group)");
                    sb.Append(")");
                }
                string filter = sb.ToString();
                SearchResultCollection resList = GetSearchResults(GetRootEntry(), filter, searchSubtrees: true);
                foreach (SearchResult groupRes in resList)
                {
                    var parentGroup = groupRes.GetDirectoryEntry().CopyTo(new LdapGroupInfo());
                    if (groups.IndexOfDN(parentGroup.DistinguishedName) == -1)
                    {
                        groups.Add(parentGroup);
                        LoadParentsOfGroup(parentGroup, groups);
                    }
                }
            }
        }

        /// <summary>
        /// Recursive method to retrieve the child groups of the spcified group.
        /// </summary>
        /// <param name="group">The parent group containing the children groups to be found.</param>
        /// <param name="groups">A collection of all of the groups that have already been added to the list.</param>
        protected void LoadChildrenOfGroup(LdapGroupInfo group, LdapGroupCollection groups)
        {
            string filter = $"(&(objectClass=group)(memberOf={group.DistinguishedName}))";
            SearchResultCollection resList = GetSearchResults(GetRootEntry(), filter, searchSubtrees: true);
            foreach (SearchResult groupRes in resList) 
            {
                var childGroup = groupRes.GetDirectoryEntry().CopyTo(new LdapGroupInfo());
                if (groups.IndexOfDN(childGroup.DistinguishedName) == -1)
                {
                    groups.Add(childGroup);
                    LoadChildrenOfGroup(childGroup, groups);
                }
            }
        }

        /// <summary>
        /// Returns a list of the groups in the specified folder (ogranizationalUnit).
        /// </summary>
        /// <param name="folderDN">The distinctive name of the folder holding the groups to be returned.
        /// <param name="includeSubFlders">Set to true to include subfolds in the search.
        public LdapGroupCollection GetGroupsInFolder(string folderDN, bool includeSubFolders = false)
        {
            var groups = new LdapGroupCollection();
            string filter = "(objectClass=group)";
            DirectoryEntry baseEntry = GetEntryByDN(folderDN);
            if (baseEntry == null) throw new NullReferenceException("Unable to find LDAP organizational unit with DN=" + folderDN ?? "");
            SearchResultCollection resList = GetSearchResults(baseEntry, filter, searchSubtrees: includeSubFolders);
            foreach (SearchResult res in resList)
            {
                var entry = res.GetDirectoryEntry();
                var group = entry.CopyTo(new LdapGroupInfo());
                groups.Add(group);
            }
            return groups;
        }

        /// <summary>
        /// Returns a list of the groups in the specified folder (ogranizationalUnit).
        /// </summary>
        /// <param name="folder">The folder holding the groups to be returned.
        /// <param name="includeSubFlders">Set to true to include subfolds in the search.
        public LdapGroupCollection GetGroupsInFolder(LdapFolderInfo folder, bool includeSubFolders = false)
        {
            return GetGroupsInFolder(folder.DistinguishedName, includeSubFolders);
        }

        /// <summary>
        /// Returns a list of all of the groups in the LDAP root entry.
        /// </summary>
        public LdapGroupCollection GetAllGroups()
        {
            return GetGroupsInFolder(_root.DN, includeSubFolders: true);
        }

        /// <summary>
        /// Retrieves a list of user accounts that are in the specified folder with the specified distinguished name.
        /// </summary>
        public LdapUserCollection GetUsersInFolder(string folderDN, bool includeSubFolders = false)
        {
            var users = new LdapUserCollection();
            string filter = "(&(objectCategory=person)(objectClass=user))";
            DirectoryEntry baseEntry = GetEntryByDN(folderDN);
            if (baseEntry == null) throw new NullReferenceException("Unable to find LDAP organizational unit with DN=" + folderDN ?? "");
            SearchResultCollection resList = GetSearchResults(baseEntry, filter, searchSubtrees: includeSubFolders);
            foreach (SearchResult res in resList)
            {
                var entry = res.GetDirectoryEntry();
                var user = entry.CopyTo(new LdapUserInfo());
                users.Add(user);
            }
            return users;
        }

        /// <summary>
        /// Retrieves a list of user accounts that are in the specified folder (organizational unit).
        /// </summary>
        public LdapUserCollection GetUsersInFolder(LdapFolderInfo folder, bool includeSubFolders = false)
        {
            return GetUsersInFolder(folder.DistinguishedName, includeSubFolders);
        }

        /// <summary>
        /// Returns a list of the folders having a parnet folder with the specified folderDistinguishedName.
        /// </summary>
        public LdapFolderCollection GetSubFolders(string folderDistinguishedName, bool includeSubFolders = false)
        {
            var folders = new LdapFolderCollection();
            string filter = "(objectClass=organizationalUnit)";
            DirectoryEntry baseEntry = GetEntryByDN(folderDistinguishedName);
            if (baseEntry == null) throw new NullReferenceException("Unable to find LDAP organizational unit with DN=" + folderDistinguishedName ?? "");
            SearchResultCollection resList = GetSearchResults(baseEntry, filter, searchSubtrees: includeSubFolders);
            foreach (SearchResult res in resList)
            {
                var entry = res.GetDirectoryEntry();
                var folder = entry.CopyTo(new LdapFolderInfo());
                folders.Add(folder);
            }
            return folders;
        }

        /// <summary>
        /// Returns a list of all of the groups in the LDAP root entry.
        /// </summary>
        public LdapFolderCollection GetSubFolders(LdapFolderInfo parentFolder, bool includeSubFolders = false)
        {
            return GetSubFolders(parentFolder.DistinguishedName, includeSubFolders);
        }

        public LdapFolderCollection GetRootFolders()
        {
            return GetSubFolders(_root.DN, includeSubFolders: false);
        }

        public LdapFolderInfo GetFolderByGuid(Guid guid)
        {
            DirectoryEntry entry = GetEntryByGuid(guid);
            if (entry == null)
                return null;
            else
                return entry.CopyTo(new LdapFolderInfo());
        }

        public LdapFolderInfo GetFolderByDN(string distinguishedName)
        {
            DirectoryEntry entry = GetEntryByDN(distinguishedName);
            if (entry == null)
                return null;
            else
                return entry.CopyTo(new LdapFolderInfo());
        }



    }
}
