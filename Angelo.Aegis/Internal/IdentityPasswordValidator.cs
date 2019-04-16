using System;
using System.Threading.Tasks;

using static IdentityModel.OidcConstants;
using IdentityServer4.Validation;
using IdentityServer4.Models;

using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Aegis.Internal
{
    public class IdentityPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly SignInManager _signInManager;
        private readonly UserManager _userManager;

        public IdentityPasswordValidator(UserManager userManager, SignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = await _userManager.FindByNameAsync(context.UserName);
            if (user != null)
            {
                if (await _signInManager.CanSignInAsync(user))
                {
                    if (_userManager.SupportsUserLockout && await _userManager.IsLockedOutAsync(user))
                    {
                        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User Locked Out");
                    }
                    else if (await _userManager.CheckPasswordAsync(user, context.Password))
                    {
                        if (_userManager.SupportsUserLockout)
                        {
                            await _userManager.ResetAccessFailedCountAsync(user);
                        }

                        var sub = await _userManager.GetUserIdAsync(user);
                        context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
                    }
                    else if (_userManager.SupportsUserLockout)
                    {
                        await _userManager.AccessFailedAsync(user);
                    }
                }
            }
        }
    }
}
