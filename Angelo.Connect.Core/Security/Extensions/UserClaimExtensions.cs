using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace Angelo.Connect.Security
{
    public static class UserClaimExtensions
    {
        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var firstName = principal.Claims.FirstOrDefault(x => x.Type == ProfileClaimTypes.FirstName)?.Value;
            var lastName = principal.Claims.FirstOrDefault(x => x.Type == ProfileClaimTypes.LastName)?.Value;
            var fullName = firstName + " " + lastName;

            return fullName.Trim();
        }

        public static string GetEmailAddress(this ClaimsPrincipal principal)
        {
            return principal.Claims.FirstOrDefault(x => x.Type == ProfileClaimTypes.Email)?.Value;
        }

        
    }
}
