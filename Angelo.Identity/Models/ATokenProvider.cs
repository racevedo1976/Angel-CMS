using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class ConnectCustomTokenProvider<TUser> : IUserTwoFactorTokenProvider<TUser> where TUser : class
    {
        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var token = "";
            if (manager != null)
            {
                token = await manager.GetUserIdAsync(user);
                
            }
            return token;
        }

        public virtual async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            var generatedToken = "";
            if (manager != null)
            {
                generatedToken = await manager.GetUserIdAsync(user);

            }
            return token == generatedToken;
        }
    }
}
