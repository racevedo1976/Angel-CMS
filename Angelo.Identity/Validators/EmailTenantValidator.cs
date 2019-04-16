using Angelo.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;

namespace Angelo.Identity.Validators
{
    public class EmailTenantValidator : IUserValidator<User>
    {
        private IdentityDbContext _identityDb;
        

        public EmailTenantValidator(
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
            
            await ValidateEmail(user, errors);
           
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

      
        // make sure email is not empty, valid, and unique by TenantId
        private async Task ValidateEmail(User user, List<IdentityError> errors)
        {

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add(Describer.InvalidEmail(user.Email));
                return;
            }
            if (!new EmailAddressAttribute().IsValid(user.Email))
            {
                errors.Add(Describer.InvalidEmail(user.Email));
                return;
            }

            if (await _identityDb.Users.AnyAsync(x => x.TenantId == user.TenantId && x.Email == user.Email && x.Id != user.Id))
            {
                errors.Add(Describer.DuplicateEmail(user.Email));
            }
        }
    }
}
