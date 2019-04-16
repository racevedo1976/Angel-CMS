using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

using Angelo.Identity.Config;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Angelo.Identity.Abstractions;

namespace Angelo.Identity
{
    public class SignInManager : SignInManager<User>
    {
        private new UserManager UserManager { get; set; }
        private PasswordHasher _passwordHasher;
        private DirectoryManager _directoryManager;
        private TenantManager _tenantManager;
        private ILdapService<LdapUser> _ldapService;
        private ISyncService<User, LdapUser> _syncService;

        public SignInManager(
            UserManager userManager,
            DirectoryManager directoryManager,
            ClaimsFactory claimsFactory,
            PasswordHasher passwordHasher,
            TenantManager tenantManager,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager> logger,
            ILdapService<LdapUser> ldapService,
            ISyncService<User, LdapUser> syncService,
            IHttpContextAccessor httpContextAccessor
        )
        : base(userManager, httpContextAccessor, claimsFactory, optionsAccessor, logger)
        {
            UserManager = userManager;

            _tenantManager = tenantManager;
            _directoryManager = directoryManager;
            _ldapService = ldapService;
            _syncService = syncService;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Override of Identity PasswordSignInAsync to validate a User's membership to a SecurityPool is active
        /// </summary>
        /// <param name="tenantKey">The TenantKey of the OIDC server</param>
        /// <param name="user">The User object</param>
        /// <param name="password">The provided unencryped password</param>
        /// <param name="poolId">The security pool to check for user membership</param>
        /// <param name="isPersistent">Flag indicating whether to persist the login cookie when the browser is closed</param>
        /// <param name="lockoutOnFailure">Flag indicating whether to lockout the user after upon reaching the invalid login attempt threshold</param>
        /// <returns></returns>
        public async Task<SignInResultCustom> PasswordSignInAsync(string tenantKey, string username, string password, bool isPersistent = false, bool lockoutOnFailure = false)
        {
            Ensure.Argument.NotNull(tenantKey);
            Ensure.Argument.NotNull(username);
            Ensure.Argument.NotNull(password);

            Tenant tenant = null;

            string seperator = null;
            if (username.Contains("/")) seperator = "/";
            if (username.Contains(@"\")) seperator = @"\";

            // Handle full qualified user names
            if(seperator != null)
            {
                var corpTenantKey = IdentityConstants.CorpTenantKey;
                var userTenantKey = username.Split(seperator.ToCharArray()).First();

                // If the tenant specified by the user is valid, sign-in using this tenant
                if (userTenantKey.ToLower() == tenantKey.ToLower() || userTenantKey.ToLower() == corpTenantKey.ToLower())
                {
                    tenant = await _tenantManager.GetByKeyAsync(userTenantKey);
                    username = username.Replace(userTenantKey + seperator, "");
                }
            }
            else
            {
                // use the tenant supplied by the request
                tenant = await _tenantManager.GetByKeyAsync(tenantKey);
            }

            if (tenant != null)
                return await TenantSignInAsync(tenant, username, password, isPersistent, lockoutOnFailure);


            // if we get here we failed
            return SignInResultCustom.Failed();
        }


        private async Task<SignInResultCustom> TenantSignInAsync(Tenant tenant, string username, string password, bool isPersistent = false, bool lockoutOnFailure = false)
        {
            var user = await UserManager.FindByNameAsync(tenant.Id, username);
            var ldapEnabled = await _directoryManager.CheckForLdapSupport(tenant);
            var passwordSuccess = false;

            // Local users will immediately succeed if the password matches
            // NOTE: Skip if password hash is null since local users can't have null passwords
            if(user != null && user.PasswordHash != null)
                passwordSuccess = VerifyPasswordHash(user, password);

            // If user could not be signed in locally then let's try ldap
            if (ldapEnabled && !passwordSuccess)
            {
                var ldapUser = await LdapSignInAsync(tenant, username, password);

                if (ldapUser != null)
                {
                    // Success, ldap user found....
                    // Sync the user and sign in using external auth scheme
                    user = await SyncUserWithLdap(user, ldapUser);

                    await SignInAsync(user, isPersistent, "Ldap");
                    return SignInResultCustom.Success(user);
                }
            }

            // fail if the user could not be found
            if (user == null)
            {
                return SignInResultCustom.Failed();
            }
          
            // Return any abnormal user states only if the user supplied the correct password
            if (passwordSuccess)
            {
                if (user.MustChangePassword)
                    return SignInResultCustom.ChangePasswordRequired(user);

                // fail if the user's email address has not been confirmed (custom)
                if (!user.EmailConfirmed)
                    return SignInResultCustom.EmailNotConfirmed();
            }


            // If we get this far then continue processing using base SignInManager to handle 
            // two factor authentication, lockout policies, and password rehashing
            var baseResult = await PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            return new SignInResultCustom(user, baseResult);
        }

        private bool VerifyPasswordHash(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (result == PasswordVerificationResult.Failed)
                return false;

            return true;
        }

        private async Task<User> SyncUserWithLdap(User user, LdapUser ldapUser)
        {
            //compare ldap and connect user 
            if ((user != null && ldapUser != null))          //user exists in both
            {
                //Sync both accounts
                //merge, updating connect details from ldap
                user = await _syncService.SyncUserFromLdapAsync(user, ldapUser);

            }
            else if ((user == null) && (ldapUser != null))  //user exists in ldap, not in connect
            {
                //sync by creating the Connect account from Ldap
                user = await _syncService.CreateUserFromLdapAsync(ldapUser);
            }

            return user;
        }

        private async Task<LdapUser> LdapSignInAsync(Tenant tenant, string username, string password)
        {
            var ldapUser = default(LdapUser);
            var directories = await _directoryManager.GetDirectoriesAsync(tenant);

            foreach (var dir in directories)
            {
                var ldapConfig = await _directoryManager.GetDirectoryLdapAsync(dir.Id);
                if (ldapConfig != null)
                {
                    _ldapService.LdapConfig = ldapConfig;
                    ldapUser = _ldapService.Login(username, password);

                    //if we have an ldapUser, then we are auth... at least that is the idea.
                    if (ldapUser != default(LdapUser))
                    {
                        //user was found in ldap, no need to continue within the loop.
                        ldapUser.DirectoryId = dir.Id;
                        ldapUser.Password = password;
                        
                        break;
                    }
                }
            }


            return ldapUser;
        }

        /// <summary>
        /// Override of Identity ExternalLoginSignInAsync to validate a User's membership to a SecurityPool is active
        /// </summary>
        /// <param name="tenantKey">The TenantKey of the OIDC server</param>
        /// <param name="loginProvider">The Pogin Provider used</param>
        /// <param name="providerKey">The user's identifier provided by the Login Provider</param>
        /// <param name="poolId">The security pool to check for user membership</param>
        /// <param name="isPersistent">Flag indicating whether to persist the login cookie when the browser is closed</param>
        /// <returns></returns>
        public async Task<SignInResultCustom> ExternalLoginSignInAsync(string tenantKey, string loginProvider, string providerKey, bool isPersistent = false)
        {
            Ensure.Argument.NotNull(tenantKey);
            Ensure.Argument.NotNull(loginProvider);
            Ensure.Argument.NotNull(providerKey);

            var tenant = await _tenantManager.GetByKeyAsync(tenantKey);
            var user = await UserManager.FindByLoginAsync(tenant?.Id, loginProvider, providerKey);

            if (user == null)
                return SignInResultCustom.Failed();

            var validTenantUser = await IsValidTenantUser(tenant.Id, user);
            if (!validTenantUser)
            {
                return SignInResultCustom.NotAllowed(user);
            }

            // else
            var result = await ExternalLoginSignInAsync(loginProvider, providerKey, isPersistent);

            return new SignInResultCustom(user, result);
        }

        //
        // Override the SignOutAsync to avoid the "no provider error" when the base method tries to execute:
        //      await Context.Authentication.SignOutAsync(Options.Cookies.TwoFactorUserIdCookieAuthenticationScheme);
        //
        public override async Task SignOutAsync()
        {
            await Context.Authentication.SignOutAsync(Options.Cookies.ApplicationCookieAuthenticationScheme);
            await Context.Authentication.SignOutAsync(Options.Cookies.ExternalCookieAuthenticationScheme);
        }


        public async Task<bool> IsValidTenantUser(string tenantId, User user)
        {
            bool isValidDirectory = false;

            // allow users from the corp directory to sign into any tenant
            if (user.DirectoryId == IdentityConstants.CorpDirectoryId)
            {
                isValidDirectory = true;
            }
            #region Backwards Compatability
            else if(user.DirectoryId == "pcmac-directory")
            {
                // TODO: Remove this once UAT data is refreshed
                isValidDirectory = true;
            }
            #endregion       
            else
            {
                // otherwise ensure the user belongs to this tenant
                var directory = await _directoryManager.GetByIdASync(user.DirectoryId);

                isValidDirectory = directory.TenantId == tenantId;
            }                   

            return isValidDirectory && user.IsActive;
        }

        private string GetCorpTenantId()
        {
            var directory = _directoryManager.GetByIdASync(IdentityConstants.CorpDirectoryId).Result;

            return directory?.TenantId;
        }


    }
}
