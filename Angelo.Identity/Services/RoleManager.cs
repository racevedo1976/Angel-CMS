using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Identity;

using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class RoleManager : RoleManager<Role>
    {

        private IdentityDbContext _db;

        public RoleManager(
            IdentityDbContext db,
            RoleStore roleStore,
            IEnumerable<IRoleValidator<Role>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<RoleManager<Role>> logger,
            IHttpContextAccessor contextAccessor
        )
        : base(roleStore, roleValidators, keyNormalizer, errors, logger, contextAccessor)
        {
            _db = db;
        }

       

        /// <summary>
        /// Gets a Role by Id
        /// </summary>
        /// <param name="roleId">The unique Id of the Role</param>
        /// <returns>The Role instance or null if not found</returns>
        public async Task<Role> GetByIdAsync(string roleId)
        {            
            Ensure.Argument.NotNullOrEmpty(roleId, "roleId");

            return await _db.Roles
                .Include(x => x.LdapMappings)
                .Include(x => x.RoleClaims)
                .Include(x => x.UserRoles).ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == roleId);
        }


        public new async Task<IdentityResult> UpdateAsync(Role role)
        {
            if (string.IsNullOrEmpty(role.Id))
                return Failure("Id is required to update the role");

            var existing = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(x => x.Id == role.Id);

            if (existing == null)
                return Failure("Cannot update a role that does not exist");

            if (existing.IsLocked)
                return Failure("Cannot update a locked role");

            return await base.UpdateAsync(role);
        }

        /// <summary>
        /// Deletes a Role by Id
        /// </summary>
        /// <param name="roleId">The unique id of the Role</param>
        /// <returns>Identity Result</returns>
        public async Task<IdentityResult> DeleteAsync(string roleId)
        {
            Ensure.Argument.NotNullOrEmpty(roleId, "roleId");

            var role = await _db.Roles.FirstOrDefaultAsync(x => x.Id == roleId);

            return await DeleteAsync(role);
        }


        /// <summary>
        /// Adds multiple users to a role 
        /// </summary>
        /// <param name="role">The role to add the claim</param>
        /// <param name="userIds">The list of userIds to add to the role</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> AddUsersAsync(Role role, IEnumerable<string> userIds)
        {
            Ensure.Argument.NotNull(role, "role");
            Ensure.Argument.NotNull(userIds, "userIds");

            IdentityResult result;
            try
            {
                foreach (var userId in userIds)
                {
                    _db.UserRoles.Add(new UserRole()
                    {
                        RoleId = role.Id,
                        UserId = userId,
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

        /// <summary>
        /// Adds multiple users to a role 
        /// </summary>
        /// <param name="role">The role to add the claim</param>
        /// <param name="userIds">The list of userIds to add to the role</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> AddUserToRoleAsync(Role role, string userId)
        {
            Ensure.Argument.NotNull(role, "role");
            Ensure.Argument.NotNull(userId, "userId");

            IdentityResult result;
            try
            {
                if (!_db.UserRoles.Any(x => x.UserId == userId && x.RoleId == role.Id))
                {
                    _db.UserRoles.Add(new UserRole()
                    {
                        RoleId = role.Id,
                        UserId = userId,
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

        /// <summary>
        /// Adds multiple users to a role 
        /// </summary>
        /// <param name="role">The role to add the claim</param>
        /// <param name="userIds">The list of userIds to add to the role</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> RemoveUsersAsync(Role role, IEnumerable<string> userIds)
        {
            Ensure.Argument.NotNull(role, "role");
            Ensure.Argument.NotNull(userIds, "userIds");

            IdentityResult result;
            try
            {
                var userRoles = _db.UserRoles.Where(x =>
                    x.RoleId == role.Id
                    && userIds.Contains(x.UserId)
                );

                _db.UserRoles.RemoveRange(userRoles);

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return await Task.FromResult(result);
        }

        /// <summary>
        /// Gets all custom claims for a role
        /// </summary>
        /// <param name="role"></param>
        public override async Task<IList<Claim>> GetClaimsAsync(Role role)
        {
            Ensure.Argument.NotNull(role, "role");

            return await GetClaimsAsync(role.Id);
        }

        /// <summary>
        /// Gets all custom claims for a role id
        /// </summary>
        /// <param name="role"></param>
        public async Task<IList<Claim>> GetClaimsAsync(string roleId)
        {
            Ensure.Argument.NotNull(roleId, "roleId");

            return await _db.RoleClaims
                .Where(x => x.RoleId == roleId)
                .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .ToListAsync();
        }

        
        /// <summary>
        /// Gets all custom claims for a role
        /// </summary>
        /// <param name="role"></param>
        public async Task<IList<RoleClaim>> GetClaimObjectsAsync(Role role)
        {
            Ensure.Argument.NotNull(role, "role");

            return await _db.RoleClaims
                .Where(x => x.RoleId == role.Id)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the specified custom role claim
        /// </summary>
        /// <param name="claimId"></param>
        public async Task<RoleClaim> GetClaimAsync(int claimId)
        {
            return await _db.RoleClaims.FirstOrDefaultAsync(x => x.Id == claimId);
        }

        /// <summary>
        /// Gets the specified custom role claim
        /// </summary>
        /// <param name="claimId"></param>
        public IQueryable<RoleClaim> QueryRoleClaims()
        {
            return _db.RoleClaims.AsQueryable();
        }

        /// <summary>
        /// Adds a security claim to a Role
        /// </summary>
        /// <param name="role">The role to add the claim</param>
        /// <param name="claim">The new claim</param>
        /// <returns>IdentityResult</returns>
        public override async Task<IdentityResult> AddClaimAsync(Role role, Claim claim)
        {
            Ensure.Argument.NotNull(role, "role");
            Ensure.Argument.NotNull(claim, "claim");
                    
            IdentityResult result;
            try
            {
                _db.RoleClaims.Add(new RoleClaim()
                {
                    RoleId = role.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });

                await _db.SaveChangesAsync();

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Update's an existing Role Claim by Id
        /// </summary>
        /// <param name="claimId">The unique id of the role claim to update</param>
        /// <param name="claim">The new claim values</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> UpdateClaimAsync(int claimId, Claim claim)
        {
            Ensure.Argument.NotNull(claim, "claim");
            Ensure.Argument.Is(claimId > 0, "claimId is required");

            var oldClaim = await _db.RoleClaims.FirstOrDefaultAsync(x => x.Id == claimId);

            if(oldClaim == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = $"Could not find Claim Id {claimId} to update." });
            }

            IdentityResult result;
            try
            {
                _db.RoleClaims.Remove(oldClaim);
                _db.RoleClaims.Add(new RoleClaim()
                {
                    RoleId = oldClaim.RoleId,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });

                await _db.SaveChangesAsync();

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Removes a Role's Claim by Id
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveClaimAsync(int claimId)
        {
            Ensure.Argument.Is(claimId > 0, "claimId is required");
            IdentityResult result;

            var claim = await _db.RoleClaims.FirstOrDefaultAsync(x => x.Id == claimId);

            if (claim == null)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Description = $"No claim with id {claimId} exists to delete."
                });
            }
            else
            {
                try
                {
                    _db.RoleClaims.Remove(claim);
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


        /// <summary>
        /// Removes a Role's Claim by Value
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveClaimAsync(string claimValue, string roleId)
        {
            Ensure.Argument.NotNull(claimValue , "claimId is required");
            Ensure.Argument.NotNull(roleId, "roleId is required");
            IdentityResult result;

            var claim = await _db.RoleClaims.FirstOrDefaultAsync(x => x.ClaimValue == claimValue
                                                                    && x.RoleId == roleId);

            if (claim == null)
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Description = $"No claim with id {claimValue} exists to delete."
                });
            }
            else
            {
                try
                {
                    _db.RoleClaims.Remove(claim);
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

        /// <summary>
        /// Removes a Role's Claim by Value
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveClaimAsync(string claimType, string claimValue, string roleId)
        {
            Ensure.Argument.NotNull(claimValue, "claimValue is required");
            Ensure.Argument.NotNull(claimType, "claimType is required");
            Ensure.Argument.NotNull(roleId, "roleId is required");
            IdentityResult result = new IdentityResult();

            var claim = await _db.RoleClaims.FirstOrDefaultAsync(x => x.ClaimValue == claimValue
                                                                    && x.ClaimType == claimType
                                                                    && x.RoleId == roleId);

            if (claim != null)
            {
                try
                {
                    _db.RoleClaims.Remove(claim);
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

        /// <summary>
        /// Gets all LDAP Mappings for a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LdapMapping>> GetLdapMappingsAsync(Role role)
        {
            Ensure.Argument.NotNull(role, "role");

            return await _db.LdapMappings.Where(x => x.RoleId == role.Id).ToListAsync();
        }

        /// <summary>
        /// Gets all LDAP Mappings for a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LdapMapping>> GetLdapMappingsAsync(string roleId)
        {
            Ensure.Argument.NotNull(roleId, "role Id");

            return await _db.LdapMappings.Where(x => x.RoleId == roleId).ToListAsync();
        }

        /// <summary>
        /// Gets all LDAP Mappings for a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<List<LdapMapping>> GetLdapMappingAsync(string roleId)
        {
            Ensure.Argument.NotNull(roleId, "role Id");

            return await _db.LdapMappings.Where(x => x.RoleId == roleId).ToListAsync();
        }

        public IQueryable<LdapMapping> QueryLdapMapping()
        {
            return _db.LdapMappings.AsQueryable();
        }
        //public IQueryable<RoleClaim> QueryRoleClaims()
        //{
        //    return _db.RoleClaims.AsQueryable();
        //}
        /// <summary>
        /// Delete all LDAP Mappings for a role
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteLdapMappingAsync(string roleId)
        {
            Ensure.Argument.NotNull(roleId, "role Id");
            var result = new IdentityResult();
            var mappings = await _db.LdapMappings.Where(x => x.RoleId == roleId).ToListAsync();

            _db.LdapMappings.RemoveRange(mappings);
            _db.SaveChanges();

            result = IdentityResult.Success;

            return result;
        }


        /// <summary>
        /// Gets a LDAP Mapping instance by Id
        /// </summary>
        /// <param name="id">The unique id of the mapping</param>
        /// <returns></returns>
        public async Task<LdapMapping> GetLdapMappingAsync(int mappingId)
        {
            Ensure.Argument.Is(mappingId >= 0, "A valid mappingId must be supplied");
            return await _db.LdapMappings.FirstOrDefaultAsync(x => x.Id == mappingId);
        }

        /// <summary>
        /// Adds a new LDAP mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public async Task<IdentityResult> AddLdapMappingAsync(LdapMapping mapping)
        {
            Ensure.Argument.NotNull(mapping, "ldapMapping");
            Ensure.Argument.Is(mapping.Id == 0, "Id cannot be supplied when adding new LDAP mappings");
            IdentityResult result;
                  
            try
            {
                _db.LdapMappings.Add(mapping);
                await _db.SaveChangesAsync();

                result = IdentityResult.Success;
            }
            catch(Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Updates an existing LDAP mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public async Task<IdentityResult> UpdateLdapMappingAsync(LdapMapping mapping)
        {
            Ensure.Argument.NotNull(mapping, "ldapMapping");

            var result = await DeleteLdapMappingAsync(mapping.Id);

            if (!result.Succeeded)
            {
                return result;
            }

            // clear the id since this will be a new entry
            mapping.Id = 0;

            return await AddLdapMappingAsync(mapping);
        }

        /// <summary>
        /// Removes an LDAP Mapping
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> DeleteLdapMappingAsync(int mappingId)
        {
            Ensure.Argument.NotNull(mappingId, "mappingId");
            IdentityResult result;

            var mapping = await GetLdapMappingAsync(mappingId);

            if(mapping == null)
            {
                result = IdentityResult.Failed(new IdentityError() {
                    Description = $"No LDAP mapping with id {mappingId} exists to delete."
                });
            }
            else
            {
                try
                {
                    _db.LdapMappings.Remove(mapping);
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

        private IdentityResult Failure(string message, string code = null)
        {
            var error = new IdentityError { Code = code, Description = message };

            return IdentityResult.Failed(error);
        }

    }
}
