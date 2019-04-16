using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Security.Claims;

namespace Angelo.Connect.Security
{
    public static class SecurityClaimExtensions
    {
        public static bool Find(this IEnumerable<SecurityClaim> sourceClaims, string type, string value)
        {
            return sourceClaims.Any(x => x.Type == type && x.Value == value);
        }

        public static bool Find(this IEnumerable<SecurityClaim> sourceClaims, Claim claim)
        {
            return sourceClaims.Any(x => x.Type == claim.Type && x.Value == claim.Value);
        }

        public static bool FindAny(this IEnumerable<SecurityClaim> sourceClaims, IEnumerable<Claim> claims)
        {
            return claims.Any(x => sourceClaims.Any(y => y.Type == x.Type && y.Value == x.Value));
        }

        public static bool FindAll(this IEnumerable<SecurityClaim> sourceClaims, IEnumerable<Claim> claims)
        {
            return claims.All(x => sourceClaims.Any(y => y.Type == x.Type && y.Value == x.Value));
        }
    }
}
