using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class SecurityPoolManager
    {
        private IdentityDbContext _db;
        private RoleManager _roleManager;
        private UserManager _userManager;

        public SecurityPoolManager(IdentityDbContext dbContext, RoleManager roleManager, UserManager userManager)
        {
            _db = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        //
        // Pool Methods
        //

        /// <summary>
        /// Checks for the existance of a SecurityPool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>True if the SecurityPool exists, false if not</returns>
        public async Task<bool> ExistsAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return await _db.SecurityPools.AsNoTracking().AnyAsync(x => x.PoolId == poolId);
        }

        /// <summary>
        /// Gets a SecurityPool by Id
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>SecurityPool instance or null if not found</returns>
        public async Task<SecurityPool> GetByIdAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return await _db.SecurityPools.FirstOrDefaultAsync(x => x.PoolId == poolId);
        }

        /// <summary>
        /// Gets a SecurityPool by Name
        /// </summary>
        /// <param name="poolName">The name of the SecurityPool</param>
        /// <returns>SecurityPool instance or null if not found</returns>
        public async Task<SecurityPool> GetByNameAsync(string poolName)
        {
            Ensure.Argument.NotNullOrEmpty(poolName, "poolName");

            return await _db.SecurityPools.FirstOrDefaultAsync(x => x.Name == poolName);
        }


        /// <summary>
        /// Creates a top level security pool for a tenant
        /// </summary>
        public async Task<SecurityPool> CreateTenantPoolAsync(string tenantKey, string name, IEnumerable<Directory> directoriesToMap)
        {
            Ensure.Argument.NotNullOrEmpty(name);
            Ensure.Argument.NotNullOrEmpty(tenantKey);

            var tenant = await _db.Tenants.FirstOrDefaultAsync(x => x.Key == tenantKey);

            if (tenant == null)
                throw new NullReferenceException($"Tenant {tenantKey} does not exist.");


            var pool = new SecurityPool
            {
                PoolType = PoolType.Client,
                PoolId = KeyGen.NewGuid(),
                TenantId = tenant.Id,
                Name = name
            };

            if(directoriesToMap != null)
            {
                pool.DirectoryMap = directoriesToMap.Select(
                    d => new DirectoryMap
                    {
                        DirectoryId = d.Id,
                        PoolId = pool.PoolId
                    }
                ).ToList();
            }
                     
            _db.SecurityPools.Add(pool);
            await _db.SaveChangesAsync();

            return pool;
        }

        /// <summary>
        /// Creates a child security pool for a tenant
        /// </summary>
        public async Task<SecurityPool> CreateChildPoolAsync(string parentPoolId, string name, IEnumerable<Directory> directoriesToMap)
        {
            Ensure.Argument.NotNullOrEmpty(name);
            Ensure.Argument.NotNullOrEmpty(parentPoolId);

            var parentPool = await _db.SecurityPools.FirstOrDefaultAsync(x => x.PoolId == parentPoolId);

            if (parentPool == null)
                throw new NullReferenceException($"Could not create child pool. Parent pool {parentPoolId} does not exist.");

            var childPool = new SecurityPool
            {
                PoolId = KeyGen.NewGuid(),
                TenantId = parentPool.TenantId,
                ParentPoolId = parentPool.PoolId,
                PoolType = PoolType.Site,
                Name = name
            };

            if (directoriesToMap != null)
            {
                childPool.DirectoryMap = directoriesToMap
                    .Select(
                        d => new DirectoryMap
                        {
                            DirectoryId = d.Id,
                            PoolId = childPool.PoolId
                        }
                    ).ToList();
            }

            _db.SecurityPools.Add(childPool);
            await _db.SaveChangesAsync();

            return childPool;
        }


        /// <summary>
        /// Creates a user pool
        /// </summary>
        public async Task UpdateAsync(SecurityPool entity)
        {
            Ensure.Argument.NotNull(entity);

            var pool = await _db.SecurityPools.FirstOrDefaultAsync(x => x.PoolId == entity.PoolId);
            if (pool == null)
                throw new Exception($"Pool not found (poolId:{entity.PoolId})");

            pool.Name = entity.Name;

            await _db.SaveChangesAsync();
        }


        /// <summary>
        /// Returuns all child pools, grandchildren pools, etc of a given user pool 
        /// </summary>
        public async Task<IEnumerable<SecurityPool>> GetDescendantPools(string poolId, bool inclusive = false)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, nameof(poolId));

            var descendantPools = new List<SecurityPool>();

            if (inclusive)
            {
                var parentPool = await _db.SecurityPools.AsNoTracking().FirstAsync(x => x.PoolId == poolId);
                descendantPools.Add(parentPool);
            }

            var childPools = await _db.SecurityPools.AsNoTracking().Where(x => x.ParentPoolId == poolId).ToListAsync();
            descendantPools.AddRange(childPools);

            //TODO: Optimize using SQL CTE Helper View
            foreach (var child in childPools)
            {
                descendantPools.AddRange(
                    await GetDescendantPools(child.PoolId)
                );
            }

            return descendantPools;
        }

        /// <summary>
        /// Returuns all parent pools of a given user pool 
        /// </summary>
        public async Task<IEnumerable<SecurityPool>> GetParentPoolsAsync(string poolId, bool inclusive = false)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, nameof(poolId));

            var parentPools = new List<SecurityPool>(); ;

            while (poolId != null)
            {
                var pool = await _db.SecurityPools
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.PoolId == poolId);

                if (pool == null)
                    break;

                parentPools.Add(pool);
                poolId = pool.ParentPoolId;
            }

            return parentPools;
        }

        //
        // Directory Methods
        //
        public async Task<IEnumerable<DirectoryMap>> GetDirectoryMapAsync(string poolId)
        {
            Ensure.Argument.NotNull(poolId, "poolId");

            return await _db.DirectoryMap.Where(x => x.PoolId == poolId).Include(x => x.Directory).ToListAsync();
        }


        //
        // User Methods
        //


        /// <summary>
        /// Checks if a User membership has been disabled
        /// </summary>
        /// <param name="poolId">The SecurityPool identifier</param>
        /// <param name="userId">The User identifier</param>
        /// <returns>Boolean result</returns>
        public async Task<bool> UserIsDisabledAsync(string poolId, string userId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");
            Ensure.Argument.NotNullOrEmpty(userId, "userId");

            return await _db.UserMemberships.AsNoTracking().AnyAsync(x =>
                    x.UserId == userId &&
                    x.PoolId == poolId &&
                    x.Disabled == true
            );
        }

        /// <summary>
        /// Disables a user membership to a pool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <param name="userId">The unique id of the User</param>
        /// <returns>True if the operation succeeded, false if not</returns>
        public async Task<bool> DisableUserAsync(string poolId, string userId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");
            Ensure.Argument.NotNullOrEmpty(userId, "userId");

            var success = false;
            var membership = new UserMembership();

            membership.UserId = userId;
            membership.PoolId = poolId;
            membership.Disabled = true;

            try
            {
                _db.UserMemberships.Attach(membership);
                _db.UserMemberships.Update(membership);
                await _db.SaveChangesAsync();

                return success = true;
            }
            catch (Exception error)
            {
                return success;
            }
        }

        /// <summary>
        /// Enables a user memberhip to a pool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <param name="userId">The unique id of the User</param>
        /// <returns>True if the operation succeeded, false if not</returns>
        public async Task<bool> EnableUserAsync(string poolId, string userId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");
            Ensure.Argument.NotNullOrEmpty(userId, "userId");

            var success = false;
            var membership = new UserMembership();

            membership.UserId = userId;
            membership.PoolId = poolId;
            membership.Disabled = false;

            try
            {
                _db.UserMemberships.Attach(membership);
                _db.UserMemberships.Update(membership);
                await _db.SaveChangesAsync();

                return success = true;
            }
            catch (Exception error)
            {
                return success;
            }
        }

        /// <summary>
        /// Returns Users for a SecurityPool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>ICollection of Users</returns>
        public async Task<IEnumerable<UserMembership>> GetUsersAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return await _db.UserMemberships
                .AsNoTracking()
                .Include(x => x.User)
                .ThenInclude(x => x.Roles)
                .ThenInclude(x => x.Role)
                .Include(x => x.UserClaims)
                .Where(x => x.PoolId == poolId)
                .ToListAsync();
        }

        public IQueryable<User> GetUsersQuery(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            var query = _db.Users.Where(x => x.Directory.DirectoryMap.Any(d => d.PoolId == poolId));

            return query;
        }
        /// <summary>
        /// Returns Users for a Client that are not in the current SecurityPool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>ICollection of Users</returns>
        public async Task<IEnumerable<UserMembership>> GetNonPoolUsersAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return await _db.UserMemberships
                .Include(x => x.User)
                .Include(x => x.UserClaims)
                .Where(x => x.PoolId != poolId)
                .ToListAsync();
        }

        public async Task<IdentityResult> DeleteUserFromPoolAsync(string poolId, string userId)
        {
            Ensure.Argument.NotNull(userId);
            Ensure.Argument.NotNull(poolId);
            IdentityResult result;

            var user = await _db.UserMemberships.FirstOrDefaultAsync(x => x.UserId == userId && x.PoolId == poolId);

            if (user == null)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Description = $"The user is not a member of that pool."
                });
            }
            else
            {
                try
                {
                    _db.UserMemberships.Remove(user);
                    await _db.SaveChangesAsync();

                    result = IdentityResult.Success;
                }
                catch (Exception ex)
                {
                    result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
                }
            }

            return result;
        }

        public async Task<UserMembership> GetUserByIdAsync(string poolId, string userId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");
            Ensure.Argument.NotNullOrEmpty(userId, "userId");

            var membership = await _db.UserMemberships
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.PoolId == poolId && x.UserId == userId);

            return membership;
        }

        public async Task<IdentityResult> UpdateUserAsync(string poolId, UserMembership user)
        {
            try
            {
                _db.UserMemberships.Attach(user);
                _db.UserMemberships.Update(user);

                await _db.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError[] {
                    new IdentityError() { Description = ex.Message }
                });
            }
        }

        public async Task<IdentityResult> AddUserAsync(string poolId, string userId)
        {
            try
            {
                Ensure.Argument.NotNull(userId);
                Ensure.Argument.NotNullOrEmpty(poolId);

                var membership = new UserMembership() { PoolId = poolId, UserId = userId };

                membership.UserRoles =
                    _db.Roles.Where(x => x.PoolId == poolId && x.IsDefault == true)
                    .Select(role => new UserRole()
                    {
                        UserId = userId,
                        RoleId = role.Id
                    }).ToList();

                _db.UserMemberships.Add(membership);
                await _db.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch(Exception ex)
            {
                return IdentityResult.Failed(new IdentityError[] {
                    new IdentityError() { Description = ex.Message }
                });
            }
        }

        //
        // Role Methods
        //
        public async Task<IdentityResult> SetUserRolesAsync(string poolId, string userId, IEnumerable<string> roleIds)
        {
            Ensure.Argument.NotNull(poolId, "poolId");
            Ensure.Argument.NotNull(userId, "userId");
            Ensure.Argument.NotNull(roleIds, "roleIds");

            IdentityResult result;

            //TODO: Wrap in single transaction
            try
            {
                _db.UserRoles.RemoveRange(
                    _db.UserRoles.Where(x => x.UserId == userId && x.Role.PoolId == poolId).ToList()
                );
                await _db.SaveChangesAsync();

                foreach (var roleId in roleIds)
                {
                    _db.UserRoles.Add(new UserRole()
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                }
                await _db.SaveChangesAsync();

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        public async Task<IList<Role>> GetUserRolesAsync(string poolId, string userId)
        {
            return await _db.UserRoles
                .Include(x => x.Role)
                .Where(x => x.UserId == userId && x.Role.PoolId == poolId)
                .Select(x => x.Role)
                .ToListAsync();
        }


        /// <summary>
        /// Gets all Roles for a SecurityPool
        /// </summary>
        /// <param name="poolId">The unique id of the SecurityPool</param>
        /// <returns>ICollection of Roles</returns>
        public async Task<IEnumerable<Role>> GetRolesAsync(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return await _db.Roles
                .Where(x => x.PoolId == poolId)
                .ToListAsync();
        }

        public IQueryable<Role> GetRolesQuery(string poolId)
        {
            Ensure.Argument.NotNullOrEmpty(poolId, "poolId");

            return _db.Roles.Where(x => x.PoolId == poolId);
        }

    }
}
