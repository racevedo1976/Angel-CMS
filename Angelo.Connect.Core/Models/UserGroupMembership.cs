using Angelo.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class UserGroupMembership
    {
        public string UserGroupId { get; set; }
        public string UserId { get; set; }
        public DateTime AddedDT { get; set; }
        public string AddedBy { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public bool AllowEmailMessaging { get; set; }
        public bool AllowSmsMessaging { get; set; }

        public UserGroup UserGroup { get; set; }
    }
}
