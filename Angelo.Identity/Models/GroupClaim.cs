using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class GroupClaim
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string ClaimType { get; set; }
        public string ClaimValue { get; set; }
    }
}
