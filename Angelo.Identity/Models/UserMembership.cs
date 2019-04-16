using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class UserMembership
    {
        public UserMembership()
        {
            MemberId = Guid.NewGuid().ToString("N");
            UserRoles = new List<UserRole>();
            UserClaims = new List<UserClaim>();
            UserSecurity = new List<UserSecurity>();
        }

        public string MemberId { get; set; }

        public string PoolId { get; set; }
      
        public string UserId { get; set; }

        public bool Disabled { get; set; }

        public SecurityPool SecurityPool { get; set; }

        public User User { get; set; }
       
        public IList<UserRole> UserRoles { get; set; } 

        public IList<UserClaim> UserClaims { get; set; }

        public IList<UserSecurity> UserSecurity { get; set; }
    }
}
