using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Security.Claims;

using Angelo.Connect.Configuration;
using Angelo.Identity;
using Angelo.Identity.Services;

namespace Angelo.Connect.Security
{
    public class UserContext
    {
        public string AuthenticationType { get;  }
        public bool IsAuthenticated { get; }
        public string Name { get; }
        public string UserId { get; }
        public IEnumerable<string> Roles { get; }
        public IEnumerable<SecurityClaim> SecurityClaims { get; }
        public ClaimsPrincipal Principal { get; }

        public bool IsCorpUser { get; }

        public UserContext(ClaimsPrincipal principal, IdentityDbContext identityDb, UserManager userManager, GroupManager groupManager)
        {
            if (principal?.Identity != null)
            {
                IsAuthenticated = principal.Identity.IsAuthenticated;
                AuthenticationType = principal.Identity.AuthenticationType;
                Name = principal.Identity.Name;
            }

            UserId = principal.FindFirst(ProfileClaimTypes.Subject)?.Value;
            Roles = principal.FindAll(ProfileClaimTypes.Role).Select(x => x.Value);
            Principal = principal;

            SecurityClaims = BuildSecurityClaims(identityDb, userManager, UserId, groupManager);

            IsCorpUser = SecurityClaims.Any(x => x.Type == KnownClaims.CorpClaimTypes.CorpUser);
        }

        private IEnumerable<SecurityClaim> BuildSecurityClaims(IdentityDbContext identityDb, UserManager userManager, string userId, GroupManager groupManager)
        {
            var allClaims = new List<SecurityClaim>();
            var roleClaims = identityDb.RoleClaims
                .Where(x => Roles.Contains(x.RoleId))
                .Select(x => new SecurityClaim(x.ClaimType, x.ClaimValue, x.ClaimType))
                .ToList();
           
            var groups = groupManager.GetUserMemberships(userId);
            if (groups != null)
            {
                foreach (var group in groups)
                {
                    allClaims.AddRange(groupManager.GetGroupClaims(group.Id).Select(x => new SecurityClaim(x.ClaimType, x.ClaimValue)).ToList());
                }
            }

            var userClaims = userManager.GetClaimObjectsAsync(userId).Result.Select(x => new SecurityClaim(x.ClaimType, x.ClaimValue));

            allClaims.AddRange(roleClaims);
            allClaims.AddRange(userClaims);

            return allClaims;
        }
    }
}
