using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;
using Angelo.Connect.Security;
using Angelo.Connect.Web.Config;
using Angelo.Identity;
using Angelo.Identity.Services;

namespace Angelo.Connect.Web
{
    public class UserContextAccessor : IContextAccessor<UserContext>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private IdentityDbContext _identityDb;
        private UserManager _userManager;
        private GroupManager _groupManager;

        public UserContextAccessor(IHttpContextAccessor httpContextAccessor, IdentityDbContext identityDb, UserManager userManager, GroupManager groupManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _identityDb = identityDb;
            _userManager = userManager;
            _groupManager = groupManager;
        }

        public UserContext GetContext()
        {
            if ((_httpContextAccessor.HttpContext) != null)
            {
                var userPrincipal = _httpContextAccessor.HttpContext.User;

                return new UserContext(userPrincipal, _identityDb, _userManager, _groupManager);
            }

            return null;
        }
 
    }
}
