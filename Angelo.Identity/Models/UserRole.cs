using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{
    public class UserRole : IdentityUserRole<string>
    {
        public UserMembership Membership { get; set; }

        public Role Role { get; set; }

        public User User { get; set; }
    }
}
