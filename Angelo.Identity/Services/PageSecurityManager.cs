using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class PageSecurityManager
    {

        private IServiceProvider _services;
        private IdentityDbContext _db;

        public PageSecurityManager(IdentityDbContext db, ILogger<UserManager<User>> logger)
        {
            _db = db;
            Ensure.NotNull(_db, "IdentityDbContext cannot be null");
        }

        public async Task<ICollection<UserSecurity>> GetPageSecurityUsersAsync(string pageId)
        {
            Ensure.Argument.NotNullOrEmpty(pageId);

            return await _db.UserSecurity
                         .Where(x => x.ResourceType == "page" && x.ResourceId == pageId)
                         .Include(y => y.User)
                         .ToListAsync();
        }

        public async Task<ICollection<RoleSecurity>> GetPageSecurityRolesAsync(string pageId)
        {
            Ensure.Argument.NotNullOrEmpty(pageId);

            return await _db.RoleSecurity
                         .Where(x => x.ResourceType == "page" && x.ResourceId == pageId)
                         .Include(x => x.Role)
                         .ToListAsync();
        }
    }

}
