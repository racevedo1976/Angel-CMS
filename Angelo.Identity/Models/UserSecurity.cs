using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class UserSecurity
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        public string ClaimType { get; set; }
        public string ResourceType { get; set; }
        public string ResourceId { get; set; }

        public User User { get; set; }
    }
}
