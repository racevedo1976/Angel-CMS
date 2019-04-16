using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Angelo.Identity.Config;
using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class ClaimsFactory : UserClaimsPrincipalFactory<User, Role>
    {

        public new UserManager UserManager { get; set; }
        public new RoleManager RoleManager { get; set; }

        public SecurityPoolManager SecurityPoolManager { get; set; }

        private IOptions<IdentityOptions> _optionsAccessor;
       
        public ClaimsFactory(
            UserManager userManager,
            RoleManager roleManager,
            SecurityPoolManager securityPoolManager,
            IOptions<IdentityOptions> optionsAccessor
        )
        : base(userManager, roleManager, optionsAccessor)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            SecurityPoolManager = securityPoolManager;

            _optionsAccessor = optionsAccessor;
        }


        public override async Task<ClaimsPrincipal> CreateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");


            //REVIEW: Consider consolidating JWTClaims 
            //TODO: Move Per Tenant IdentityOptions class from Angelo.Aegis to Angelo.Identity

            var authScheme = Options.Cookies.ApplicationCookie.AuthenticationScheme;
            var identity = new ClaimsIdentity(authScheme, ClaimType.Name, ClaimType.Role);
            var userId = await UserManager.GetUserIdAsync(user);
            
            identity.AddClaim(new Claim(ClaimType.Subject, userId));
            identity.AddClaim(new Claim(ClaimType.Id, userId));
            identity.AddClaim(new Claim(ClaimType.Name, await UserManager.GetUserNameAsync(user)));
            identity.AddClaim(new Claim(ClaimType.UserName, await UserManager.GetUserNameAsync(user)));
            identity.AddClaim(new Claim(ClaimType.AuthenticationTime, ToEpochTime(DateTime.UtcNow).ToString()));
            identity.AddClaim(new Claim(ClaimType.IdentityProvider, IdentityConstants.IdentityProvider));

            //TODO: refactor hardcoded values to pull from user settings
            identity.AddClaim(new Claim(ClaimType.Locale, "en-US"));
            identity.AddClaim(new Claim(ClaimType.TimeZone, "America/Chicago"));

            //TODO: Move active / disabled status to user account        
            /*           
                if (poolId != null)
                {
                    bool activeMembership = !await SecurityPoolManager.UserIsDisabledAsync(pool.PoolId, user.Id);

                    if (activeMembership)
                    {
                        // enforce pool inheritance from parent pools             
                        var parentPools = await SecurityPoolManager.GetParentPoolsAsync(poolId, inclusive: true);

                        foreach(var pool in parentPools)
                        {
                            identity.AddClaim(new Claim(ClaimType.MembershipPool, pool.PoolId));
                        }
                        
                    }
                }
            */

            //TODO: Filter roles by security pools mapped to the user's director7y
            var roles = await UserManager.GetRolesAsync(user);
            foreach (var roleId in roles)
            {
                identity.AddClaim(new Claim(ClaimType.Role, roleId));
                identity.AddClaims(await RoleManager.GetClaimsAsync(roleId));
            }

            // Custom Claims
            identity.AddClaims(await UserManager.GetClaimsAsync(user));


            // System Claims 
            var stamp = await UserManager.GetSecurityStampAsync(user);
            if (!String.IsNullOrEmpty(stamp))
            {
                identity.AddClaim(new Claim(ClaimType.SecurityStamp, stamp));
            }
                
            var externalLogins = await UserManager.GetLoginsAsync(user);
            foreach (var login in externalLogins)
            {
                identity.AddClaim(new Claim(ClaimType.ExternalLoginProvider, login.LoginProvider));
            }
           
            var email = await UserManager.GetEmailAsync(user);
            if (!String.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(ClaimType.Email, email));
                identity.AddClaim(new Claim(ClaimType.EmailConfirmed, Convert.ToString(user.EmailConfirmed), ClaimValueTypes.Boolean));
            }           

           
            var phone = await UserManager.GetPhoneNumberAsync(user);
            if (!String.IsNullOrEmpty(phone))
            {
                identity.AddClaim(new Claim(ClaimType.PhoneNumber, phone));
                identity.AddClaim(new Claim(ClaimType.PhoneNumberConfirmed, Convert.ToString(user.PhoneNumberConfirmed), ClaimValueTypes.Boolean));
            }
            
            var lockoutDate = UserManager.GetLockoutEndDateAsync(user).Result;
            if (lockoutDate.HasValue) {
                identity.AddClaim(new Claim(ClaimType.LockoutEnd, lockoutDate.Value.ToString(), ClaimValueTypes.Date));
            }

            return new ClaimsPrincipal(identity);
        }

        private static long ToEpochTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);
        }
    }
}
