using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class SecurityPermissionExtensions
    {
        public static List<SecurityClaim> FindClaims(this IEnumerable<Permission> groups, string title)
        {
            List<SecurityClaim> claims = new List<SecurityClaim>();

            foreach (var permission in groups)
            {

                claims.AddRange(GetClaims(permission, title, false));

            }

            return claims;
        }

        private static IEnumerable<SecurityClaim> GetClaims(Permission permission, string title, bool select)
        {
            List<SecurityClaim> claims = new List<SecurityClaim>();

            if (permission.Permissions.Count > 0)
            {
                if (!select)
                    select = title == permission.Title;

                foreach (var item in permission.Permissions)
                {
                    claims.AddRange(GetClaims(item, title, select));
                }

            }

            if (title == permission.Title || select)
            {
                claims.AddRange(permission.Claims);
            }

            return claims;
        }

    }
}
