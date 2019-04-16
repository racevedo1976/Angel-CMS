using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Angelo.Identity.Models;

namespace Angelo.Identity.Services
{
    public class DirectoryManager
    {
        private IdentityDbContext _db;

        public DirectoryManager(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<Directory> GetByIdASync(string directoryId)
        {
            Ensure.Argument.NotNullOrEmpty(directoryId);

            return await _db.Directories.FirstOrDefaultAsync(x => x.Id == directoryId);
        }

        public async Task<Directory> CreateAsync(string tenantKey, string directoryName)
        {
            Ensure.Argument.NotNullOrEmpty(tenantKey);
            Ensure.Argument.NotNullOrEmpty(directoryName);

            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");

            var directory = new Directory
            {
                Id = KeyGen.NewGuid(),
                Name = directoryName,
                TenantId = tenant.Id
            };

            _db.Directories.Add(directory);
            await _db.SaveChangesAsync();

            return directory;
        }

        public async Task<ICollection<Directory>> GetDirectoriesAsync(string tenantKey)
        {
            Ensure.Argument.NotNullOrEmpty(tenantKey);

            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            return await GetDirectoriesAsync(tenant);                   
        }

        public async Task<ICollection<Directory>> GetDirectoriesAsync(Tenant tenant)
        {
            if (string.IsNullOrEmpty(tenant?.Id))
                throw new NullReferenceException("Cannot get directories. Null or invalid tenant");

            return await _db.Directories
                .Where(x => x.TenantId == tenant.Id)
                .ToListAsync();
        }


        public async Task<ICollection<Directory>> GetDirectoriesWithMapAsync(string tenantKey)
        {
            Ensure.Argument.NotNullOrEmpty(tenantKey);

            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");

            return await _db.Directories
                .Include(x => x.DirectoryMap)
                .Where(x => x.TenantId == tenant.Id)
                .ToListAsync();

        }
  
        public async Task<ICollection<DirectoryMap>> GetDirectoryPoolsAsync(string directoryId)
        {

            Ensure.Argument.NotNullOrEmpty(directoryId);

            var directory = await _db.Directories
                         .Include(x => x.DirectoryMap)
                         .FirstOrDefaultAsync(x => x.Id == directoryId);

            return directory.DirectoryMap.ToList();

        }


        public async Task<LdapDomain> GetDirectoryLdapAsync(string directoryId)
        {

            Ensure.Argument.NotNullOrEmpty(directoryId);

            var ldapDomain = await _db.LdapDomains
                         .FirstOrDefaultAsync(x => x.DirectoryId == directoryId);

            return ldapDomain;

        }

        public async Task<bool> CheckForLdapSupport(Tenant tenant)
        {
            return await _db.Directories.AnyAsync(x =>
                x.TenantId == tenant.Id && x.LdapDomain.Host != null
            );
        }


        public async Task<ICollection<Directory>> GetMappedDirectoriesAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId);

            return await _db.DirectoryMap
                         .Include(x => x.Directory)
                         .Where(x => x.PoolId == poolId)
                         .Select(x => x.Directory)
                         .ToListAsync();
        }

        public async Task<Directory> GetDefaultMappedDirectoryAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId);

            var firstMapped = await _db.DirectoryMap
               .Include(x => x.Directory)
               .FirstOrDefaultAsync(x => x.PoolId == poolId);

            return firstMapped?.Directory;               
        }

        /// <summary>
        /// Returns Users for a SecurityPool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>ICollection of Users</returns>
        public async Task<IEnumerable<User>> GetUsersAsync(string directoryId)
        {
            Ensure.Argument.NotNullOrEmpty(directoryId, "directoryId");

            return await _db.Users
                .AsNoTracking()
                .Include(x => x.Security)
                .Where(x => x.DirectoryId == directoryId)
                .ToListAsync();
        }

        public async Task<User> GetUserAsync(string userId, string directoryId)
        {
            Ensure.Argument.NotNullOrEmpty(userId, "userId");
            Ensure.Argument.NotNullOrEmpty(directoryId, "directoryId");

            return await _db.Users
                .AsNoTracking()
                .Include(x => x.Security)
                .Where(x => x.DirectoryId == directoryId && x.Id == userId)
                .FirstOrDefaultAsync();
        }


        public async Task<bool> IsMapped(string directoryId, string poolId)
        {
            return await _db.DirectoryMap.AnyAsync(x => x.PoolId == poolId && x.DirectoryId == directoryId);
        }

        public void UpdateDirectoryLdapAsync(LdapDomain ldap)
        {
            _db.Update(ldap);
            _db.SaveChanges();
        }

        public void SaveDirectoryLdapAsync(LdapDomain ldap)
        {
            _db.LdapDomains.Add(ldap);
            _db.SaveChanges();
        }
    }
}
