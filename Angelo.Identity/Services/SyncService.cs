using Angelo.Identity.Abstractions;
using Angelo.Identity.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Identity.Services
{
    public class SyncService<TUser, LdUser> : ISyncService<TUser, LdUser> 
        where TUser: User, new()
        where LdUser: ILdapUser
    {

        UserManager _userManager;
        private DirectoryManager _directoryManager;
        private SecurityPoolManager _securityPoolManager;
        private RoleManager _roleManager;

        public SyncService(UserManager userManager, DirectoryManager directoryManager, SecurityPoolManager securityPoolManager, RoleManager roleManager)
        {
            _userManager = userManager;
            _directoryManager = directoryManager;
            _securityPoolManager = securityPoolManager;
            _roleManager = roleManager;
        }

        public async Task<TUser> CreateUserFromLdapAsync(LdUser ldapUser)
        {
            //extra check using the Guid, just in case user profile just changed on ldap only but needs to sync with Connect
            var userInDb = await _userManager.GetUserByLdapGuidAsync(ldapUser.LdapGuid.ToString());

            if (userInDb != null)
            {
                return await SyncUserFromLdapAsync((TUser)userInDb, ldapUser);
            }

            var user = new TUser();

            user.Id = Guid.NewGuid().ToString("N");
            user.Email = ldapUser.Email;
            user.EmailConfirmed = true;
            user.UserName = ldapUser.Username;
            user.DirectoryId = ldapUser.DirectoryId;
            user.LdapGuid = ldapUser.LdapGuid.ToString();
            //user.PasswordHash = ldapUser.Password;    Note: Never save passwords from LDAP
            user.AccessFailedCount = 0;
            user.LockoutEnabled = false;
            user.IsActive = true;
            user.PhoneNumberConfirmed = false;
            user.TwoFactorEnabled = false;
            user.FirstName = ldapUser.FirstName;
            user.LastName = ldapUser.LastName;
            user.DisplayName = ldapUser.DisplayName;
            user.Title = "";
            user.Suffix = "";
            user.BirthDate = DateTime.MinValue;
            user.LockoutEnabled = false;

            await _userManager.CreateAsync(user);

            //update groups-roles
            await SyncUserRolesFromLdapGroups(user, ldapUser);

            return user;
        }

        public async Task<TUser> SyncUserFromLdapAsync(TUser user, LdUser ldapUser)
        {

            //update user profile
            user.Email = ldapUser.Email;
            user.EmailConfirmed = true;
            user.UserName = ldapUser.Username;
            user.LdapGuid = ldapUser.LdapGuid.ToString();
            //user.PasswordHash = ldapUser.Password;     Note: Never save passwords from LDAP
            user.FirstName = ldapUser.FirstName;
            user.LastName = ldapUser.LastName;
            user.DisplayName = ldapUser.DisplayName;
            user.Email = ldapUser.Email;
            
            await _userManager.UpdateAsync(user);
            
            //update sync user roles-groups
            await SyncUserRolesFromLdapGroups(user, ldapUser);
            
            return user;

        }

        private async Task  SyncUserRolesFromLdapGroups(TUser user, LdUser ldapUser)
        {
            var allRoles = new List<Role>();

            //Find all roles for a directory. Meaning we need to query all pools to find all roles
            var pools = await _directoryManager.GetDirectoryPoolsAsync(user.DirectoryId);
            foreach (var pool in pools)
            {
                allRoles.AddRange(await _securityPoolManager.GetRolesAsync(pool.PoolId));
            }
            
            //Get only roles that were mapped from all roles founded.
            var rolesMapped = _roleManager.QueryLdapMapping().Include(x => x.Role).Where(x => allRoles.Exists(y => y.Id == x.RoleId)).ToList();

            if (!rolesMapped.Any())
                return;

            //Remove all roles from user that are mapped ONLY from the user-roles relationship 
            foreach (var mappedRole in rolesMapped)
            {
                //this should clear all roles for the user for clean insert. Should only clear roles that are mapped to Ldap groups
                await _userManager.RemoveFromRoleAsync(user, mappedRole.Role);
            }
 
            //find the ldap roles assigned to the ldapuser that are listed in the collection of mapped roles
            var rolesGroupAssignedInLdap = rolesMapped.Where(x => ldapUser.Groups.Any(y => y == x.DistinguishedName)).ToList();

            if (!rolesGroupAssignedInLdap.Any())
                return;

            //add all group/roles from the Ldap user that were found in the mapped list.
            foreach (var groupRole in rolesGroupAssignedInLdap)
            {
                await _roleManager.AddUserToRoleAsync(groupRole.Role, user.Id);
            }

            
        }
    }
}
