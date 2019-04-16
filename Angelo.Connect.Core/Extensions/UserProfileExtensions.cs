using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System.Security;
using System.Security.Claims;

namespace Angelo.Connect.Extensions
{
    public static class UserProfileExtensions
    {
        public static void MapClaims(this UserProfile profile, IEnumerable<Claim> claims)
        {
            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            profile.FullName = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.Name)?.Value;
            profile.DisplayName = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.DisplayName)?.Value;
            profile.FirstName = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.FirstName)?.Value;
            profile.LastName = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.LastName)?.Value;
            profile.Gender = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.Gender)?.Value;
            profile.Title = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.Title)?.Value;
            profile.Suffix = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.Suffix)?.Value;

            var birthday = claims.FirstOrDefault(x => x.Type == Security.ProfileClaimTypes.Suffix)?.Value;

            if (birthday != null)
                profile.BirthDate = DateTime.Parse(birthday);
        }
    }
}
