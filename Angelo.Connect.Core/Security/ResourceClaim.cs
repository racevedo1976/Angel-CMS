using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class ResourceClaim
    {

        public string ResourceType { get; set;}
        public string ResourceId { get; set; }
        public string ClaimType { get; set; }
        public string UserId { get; set; }
    }
}
