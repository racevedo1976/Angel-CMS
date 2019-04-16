using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Identity.Models;

namespace Angelo.Connect.Models
{
    public class UserProfile
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Suffix { get; set; }
        public string DisplayName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }

        public List<UserClaim> UserClaims { get; set; }
        public List<Role> Roles { get; set; }

        public UserProfile()
        {
            UserClaims = new List<UserClaim>();
            Roles = new List<Role>();
        }

    }
}
