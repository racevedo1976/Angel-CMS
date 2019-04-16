using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class UserSitePoolMemberships
    {
        public string UserId { get; set; }
        public string PoolId { get; set; }
        public string SiteId { get; set; }
        public string PageId { get; set; }
    }
}
