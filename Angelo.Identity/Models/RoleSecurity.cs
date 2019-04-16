using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class RoleSecurity
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string ClaimType { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }
        
        public Role Role { get; set; }
    }
}
