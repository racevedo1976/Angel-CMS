using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{
    public class Role : IdentityRole<string, UserRole, RoleClaim>
    {
        public Role()
        {
            Id = Guid.NewGuid().ToString("N");
            RoleClaims = new List<RoleClaim>();
            UserRoles = new List<UserRole>();
            LdapMappings = new List<LdapMapping>();
            IsLocked = false;
            IsDefault = false;
        }

        public override string Id { get; set; }
        public override string Name { get; set;}     
        public bool IsLocked { get; set; }
        public bool IsDefault { get; set; }

        public string PoolId { get; set; }
        public string Path { get; set; }
      
        public ICollection<RoleClaim> RoleClaims { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }

        public ICollection<LdapMapping> LdapMappings { get; set; }

        public SecurityPool SecurityPool { get; set; }
    }
}
