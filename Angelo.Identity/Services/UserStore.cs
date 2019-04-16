using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Angelo.Identity.Models;

namespace Angelo.Identity
{

    
    public class UserStore : UserStore<User,Role,IdentityDbContext,string,UserClaim,UserRole,UserLogin,IdentityUserToken<string>,RoleClaim>
    {

        private IdentityDbContext _db;

        public UserStore(IdentityDbContext dbContext, IdentityErrorDescriber describer) : base(dbContext, describer)
        {
            _db = dbContext;
        }

        protected override UserRole CreateUserRole(User user, Role role)
        {
            return new UserRole()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
        }

        protected override UserClaim CreateUserClaim(User user, Claim claim)
        {
            var userClaim = new UserClaim { UserId = user.Id };

            userClaim.InitializeFromClaim(claim);

            return userClaim;
        }

        protected override UserLogin CreateUserLogin(User user, UserLoginInfo login)
        {
            return new UserLogin
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        protected override IdentityUserToken<string> CreateUserToken(User user, string loginProvider, string name, string value)
        {
            return new IdentityUserToken<string>
            {
                UserId = user.Id,
                LoginProvider = loginProvider,
                Name = name,
                Value = value
            };
        }
    }
    
}
