using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;

using Angelo.Identity.Models;
using Angelo.Identity.Validators;

namespace Angelo.Identity
{
    public class UserManager : UserManager<User>
    {

        private IServiceProvider _services;
        private IdentityDbContext _db;
        
        public UserManager(
            UserStore userStore,
            IOptions<IdentityOptions> options,
            PasswordHasher passwordHasher,
            IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<UserManager<User>> logger
            //UserWithTenantValidator userWithTenantValidator,
            //EmailTenantValidator emailTenantValidator
        )
        : base(userStore, options, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _services = services;
            _db = (IdentityDbContext)_services.GetService(typeof(IdentityDbContext));
           
            Ensure.NotNull(_db, "IdentityDbContext cannot be null");
        }

        private static IEnumerable<IUserValidator<User>> GetValidators(params IUserValidator<User>[] validators)
        {
            return validators.ToList();
        }

        public new async Task<IdentityResult> CreateAsync(User user)
        {
            Ensure.NotNull(user.UserName);
            Ensure.NotNull(user.DirectoryId);
            Ensure.NotNull(user.Email);

            if (user.TenantId == null)
            {
                var directory = await _db.Directories.FirstOrDefaultAsync(x => x.Id == user.DirectoryId);
                user.TenantId = directory.TenantId;
            }

            // logic for validating on usernames and emails per tenant were extracted to their own validator class
            // see the custom Angelo.Identity.Validators classes
            
            // Use custom guid & security stamp generator
            if (user.Id == null)
                user.Id = KeyGen.NewGuid();

            if (user.SecurityStamp == null)
                user.SecurityStamp = KeyGen.NewGuid32();

            user.NormalizedEmail = user.Email.ToLower();
            user.NormalizedUserName = user.UserName.ToLower();

            return await base.CreateAsync(user);
        }



        public new async Task<IdentityResult> CreateAsync(User user, string password)
        {
            Ensure.NotNull(user.UserName);
            Ensure.NotNull(user.DirectoryId);
            Ensure.NotNull(user.Email);

            if (user.TenantId == null)
            {
                var directory = await _db.Directories.FirstOrDefaultAsync(x => x.Id == user.DirectoryId);
                user.TenantId = directory.TenantId;
            }

            // logic for validating on usernames and emails per tenant were extracted to their own validator class
            // see the custom Angelo.Identity.Validators classes
            
            // Use custom guid & security stamp generator
            if (user.Id == null)
                user.Id = KeyGen.NewGuid();

            if (user.SecurityStamp == null)
                user.SecurityStamp = KeyGen.NewGuid32();

            user.NormalizedEmail = user.Email.ToLower();
            user.NormalizedUserName = user.UserName.ToLower();
            
            return await base.CreateAsync(user, password);
        }


        public async Task<User> GetUserAsync(string userId)
        {
            var user = await _db.Users
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetUsersAsync(List<string> userIds)
        {
            var users = await _db.Users
                .Where(x => userIds.Contains(x.Id))
                .ToListAsync();
            return users;
        }

        public async Task<User> GetUserByLdapGuidAsync(string ldapGuid)
        {
            var user = await _db.Users
                .Where(x => x.LdapGuid == ldapGuid)
                .FirstOrDefaultAsync();
            return user;
        }


        public new async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _db.UserRoles
                .Include(x => x.Role)
                .Where(x => x.UserId == user.Id)
                .Select(x => x.Role.Id).ToListAsync();
        }

        public new async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _db.UserClaims
                .Where(x => x.UserId == user.Id)
                .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .ToListAsync();
        }

        public async Task<IList<UserClaim>> GetClaimObjectsAsync(User user)
        {
            return await _db.UserClaims
               .Where(x => x.UserId == user.Id)
               .ToListAsync();
        }

        public async Task<IList<UserClaim>> GetClaimObjectsAsync(string userId)
        {
            return await _db.UserClaims
               .Where(x => x.UserId == userId)
               .ToListAsync();
        }

        public async Task<IList<UserClaim>> GetClaimObjectsAsync(Claim claim)
        {
            return await _db.UserClaims
               .Where(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value)
               .ToListAsync();
        }

        public async Task<UserClaim> GetClaimObjectAsync(int claimId)
        {
            return await _db.UserClaims.FirstOrDefaultAsync(x => x.Id == claimId);
        }

        public IQueryable<UserClaim> QueryUserClaims()
        {
            return _db.UserClaims.AsQueryable();
        }

        public async Task<User> FindByLoginAsync(string tenantId, string loginProvider, string providerKey)
        {
            var login = await _db.UserLogins
                .Include(x => x.User)
                .FirstOrDefaultAsync(x =>
                    x.LoginProvider == loginProvider
                    && x.ProviderKey == providerKey
                    && x.User.Directory.TenantId == tenantId
                );

            return login?.User;                
        }

        public async Task<User> FindByNameAsync(string tenantId, string username)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.UserName == username);
        }

        /// <summary>
        /// Adds a custom claim to a User
        /// </summary>
        /// <param name="role">The user to add the claim</param>
        /// <param name="claim">The new claim</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> AddClaimAsync(string userId, Claim claim, string claimScope = null)
        {
            Ensure.Argument.NotNull(userId);
            Ensure.Argument.NotNull(claim);

            IdentityResult result;
            try
            {
                var existingClaim = await _db.UserClaims.FirstOrDefaultAsync(x => 
                    x.ClaimType == claim.Type 
                    && x.ClaimValue == claim.Value
                    && x.UserId == userId
                );

                if (existingClaim == null)
                {
                    _db.UserClaims.Add(new UserClaim()
                    {
                        UserId = userId,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value,
                        ClaimScope = claimScope
                    });

                    await _db.SaveChangesAsync();
                }               

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        /// <summary>
        /// Updates an existing custom user claim
        /// </summary>
        /// <param name="claimId">The unique id of the user claim to update</param>
        /// <param name="claim">The new claim values</param>
        /// <returns>IdentityResult</returns>
        public async Task<IdentityResult> UpdateClaimAsync(int claimId, Claim claim)
        {
            Ensure.Argument.NotNull(claim, "claim");
            Ensure.Argument.Is(claimId > 0, "claimId is required");

            var oldClaim = await _db.UserClaims.FirstOrDefaultAsync(x => x.Id == claimId);

            if (oldClaim == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = $"Could not find Claim Id {claimId} to update." });
            }

            IdentityResult result;
            try
            {
                _db.UserClaims.Remove(oldClaim);
                _db.UserClaims.Add(new UserClaim()
                {
                    UserId = oldClaim.UserId,
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
        /// Removes a custom user claim
        /// </summary>
        /// <param name="mappingId"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RemoveClaimAsync(int claimId)
        {
            Ensure.Argument.Is(claimId > 0, "claimId is required");
            IdentityResult result;

            var claim = await _db.UserClaims.FirstOrDefaultAsync(x => x.Id == claimId);

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
                    _db.UserClaims.Remove(claim);
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
        
        public async Task<IdentityResult> RemoveClaimAsync(string claimType, string claimValue, string userId)
        {
            var c = await _db.UserClaims.FirstOrDefaultAsync(x => x.UserId == userId &&
                                                                x.ClaimType == claimType &&
                                                                x.ClaimValue == claimValue);

            if (c != null)
            {
                return await RemoveClaimAsync(c.Id);
            }
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, Role role)
        {
            if (user == null || user.Id == null)
                return Failure("Cannot upate user roles. User or Id cannot be null.");
            if (role == null || role.Id == null)
                return Failure("Cannot upate user roles. Role or Id cannot be null.");

            var existingUserRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == user.Id);

            if (existingUserRole != null)
            {
                _db.UserRoles.Remove(existingUserRole);
                _db.SaveChanges();

            }


            return IdentityResult.Success;

        }

        public new async Task<IdentityResult> UpdateAsync(User user)
        {
            if (user == null || user.Id == null)
                return Failure("Cannot upate user. User or Id cannot be null.");

            // Get a non tracked version or else will throw error in base.UpdateAsync
            var userUpdate = await _db.Users.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (userUpdate == null)
                return Failure("Cannot update user. User does not exist.");

            // we're intercepting the base.update to control which fields are allowed to be updated
            userUpdate.FirstName = user.FirstName;
            userUpdate.LastName = user.LastName;
            userUpdate.Title = user.Title;
            userUpdate.UserName = user.UserName;
            userUpdate.Suffix = user.Suffix;
            userUpdate.DisplayName = user.DisplayName;
            userUpdate.PhoneNumber = user.PhoneNumber;
            userUpdate.WirelessProviderId = user.WirelessProviderId;
            userUpdate.Email = user.Email;
            userUpdate.IsActive = user.IsActive;
            userUpdate.BirthDate = user.BirthDate;
            userUpdate.MustChangePassword = user.MustChangePassword;

            return await base.UpdateAsync(userUpdate);
        }

        private IdentityResult Failure(string message, string code = null)
        {
            var error = new IdentityError { Code = code, Description = message };

            return IdentityResult.Failed(error);
        }

        public async Task<WirelessProvider> GetWirelessProviderAsync(string id)
        {
            return await _db.WirelessProviders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<WirelessProvider>> GetWirelessProvidersAsync()
        {
            return await _db.WirelessProviders.OrderBy(x => x.Name).ToListAsync();
        }
    }

}
