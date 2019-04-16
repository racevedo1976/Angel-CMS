using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Angelo.Connect.Models;
using Angelo.Connect.Extensions;
using Angelo.Identity;

namespace Angelo.Connect.Services
{
    public class UserProfileManager
    {
        private UserManager _userManager;
        private ClientManager _clientManager;
        private Identity.SecurityPoolManager _poolManager;

        public UserProfileManager(ClientManager clientManager, UserManager userManager, Identity.SecurityPoolManager poolManager)
        {
            _userManager = userManager;
            _clientManager = clientManager;
            _poolManager = poolManager;
        }

        public async Task<UserIdentity> GetIdentityAsync(string userId)
        {
            return await Task.FromResult(default(UserIdentity));
        }

        public async Task<UserProfile> GetProfileAsync(string poolId, string userId)
        {
            var userProfile = new UserProfile();

            var membership = await _poolManager.GetUserByIdAsync(poolId, userId);
            if (membership == null)
                throw new Exception($"Unable to find user [{userId}] in pool [{poolId}].");

            userProfile.FirstName = membership.User.UserName;

            // To Do: Load the claims and roles into the profile.

            return userProfile;
        }

        

       
    }
}
