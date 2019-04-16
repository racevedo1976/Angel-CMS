using Angelo.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace Angelo.Identity.Validators
{
    public class UserWithTenantValidator : IUserValidator<User>
    {
        private IdentityDbContext _identityDb;

        public UserWithTenantValidator(
            IServiceProvider services,
            IdentityErrorDescriber errors = null)
        {

            _identityDb = (IdentityDbContext)services.GetService(typeof(IdentityDbContext));
            Describer = errors ?? new IdentityErrorDescriber();

        }

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to provider error messages for the current <see cref="UserValidator{TUser}"/>.
        /// </summary>
        /// <value>The <see cref="IdentityErrorDescriber"/> used to provider error messages for the current <see cref="UserValidator{TUser}"/>.</value>
        public IdentityErrorDescriber Describer { get; private set; }

        public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
        {
            Ensure.NotNull(user);

            var errors = new List<IdentityError>();
            await ValidateUserName(user, errors);
          
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task ValidateUserName(User user, ICollection<IdentityError> errors)
        {
           
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                errors.Add(Describer.InvalidUserName(user.UserName));
            }
            else
            {
                
                if (await _identityDb.Users.AnyAsync(x => x.UserName == user.UserName && x.TenantId == user.TenantId && x.Id != user.Id))
                {
                    errors.Add(Describer.DuplicateUserName(user.UserName));
                }
            }
        }

       
    }
}
