using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{

    public class User : IdentityUser<string, UserClaim, UserRole, UserLogin>
    {
        public User() : base()
        {
            Memberships = new List<UserMembership>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Title { get; set; }
        public string Suffix { get; set; }
        public string DisplayName { get; set; }

        public string DirectoryId { get; set; }

        public string TenantId { get; set; }

        public bool IsActive { get; set; }

        public bool MustChangePassword { get; set; }

        public string WirelessProviderId { get; set; }

        public string LdapGuid { get; set; }

        public Directory Directory { get; set; }

        public Tenant Tenant { get; set; }

        public ICollection<UserMembership> Memberships { get; set; }

        public ICollection<UserSecurity> Security { get; set; }
        public ICollection<GroupMembership> GroupMemberships { get; set; }

        public bool IsLockedOut
        {
            get { return !(LockoutEnd == null || LockoutEnd.Value < DateTime.Now); }
        }
    }

}
