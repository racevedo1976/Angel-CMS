using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class SecurityPool
    {
        public SecurityPool()
        {
            Roles = new List<Role>();
            Memberships = new List<UserMembership>();
            ChildPools = new List<SecurityPool>();
        }

        public string PoolId { get; set; }

        public string Name { get; set; }

        public string TenantId { get; set; }

        public string ParentPoolId { get; set; }

        public PoolType PoolType { get; set; }


        public ICollection<DirectoryMap> DirectoryMap { get; set; }

        public ICollection<Role> Roles { get; set; }

        public ICollection<UserMembership> Memberships { get; set; }

        public SecurityPool ParentPool { get; set; }

        public ICollection<SecurityPool> ChildPools { get; set; }

        public Tenant Tenant { get; set; }
        public string LdapFilterGroup { get; set; }
    }
}
