using Angelo.Connect.Services;
using Angelo.Identity;
using Angelo.Identity.Abstractions;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Angelo.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Jobs
{
    public class JobImportLdapUsers : IJob
    {
        private ClientManager _clientManager;
        DirectoryManager _directoryManager;
        private ILdapService<LdapUser> _ldapService;
        private ISyncService<User, LdapUser> _syncService;
        private UserManager _userManager;
        private const string _userFilter = "(objectClass=user)";

        public JobImportLdapUsers(ClientManager clientManager, 
            DirectoryManager directoryManager,
            ILdapService<LdapUser> ldapService,
            ISyncService<User, LdapUser> syncService,
            UserManager userManager)
        {
            _clientManager = clientManager;
            _directoryManager = directoryManager;
            _userManager = userManager;
            _ldapService = ldapService;
            _syncService = syncService;
        }
        public async Task ExecuteAsync()
        {

            //basically, for all clients, all directories, get ldap user based on filter then
            //import ones not in the system. Nothing else.

            //Get all clients
            var clients = await _clientManager.GetAll();

            foreach (var client in clients)
            {

                //Get directories for client
                var directories = await _directoryManager.GetDirectoriesAsync(client.Id);

                foreach (var directory in directories)
                {
                    //Get Ldap info for directory
                    var ldapDomain = await _directoryManager.GetDirectoryLdapAsync(directory.Id);

                    if (ldapDomain == null)
                        break;

                    //find all users from the ldap based on filter
                    _ldapService.LdapConfig = ldapDomain;
                    var ldapUsers = _ldapService.SearchUsersPaged(_userFilter);
                    

                    //process all the ldap users returned. 
                    ProcessUsersFromLdap(ldapUsers);

                }
            }

            throw new NotImplementedException();
        }

        private async void ProcessUsersFromLdap(List<ILdapUser> ldapUsers)
        {
            if (ldapUsers != null)
            {
                if (ldapUsers.Any())
                {
                    //for each ldpa user, search the Identity Db, if not found, then add.
                    foreach (var ldapUser in ldapUsers)
                    {
                        var user = _userManager.GetUserByLdapGuidAsync(ldapUser.LdapGuid.ToString());

                        if (user == null)   //user not in Identity database, lets create this one.
                        {
                            await _syncService.CreateUserFromLdapAsync((LdapUser)ldapUser);
                        }
                    }
                }
                     
            }
        }

    }
}
