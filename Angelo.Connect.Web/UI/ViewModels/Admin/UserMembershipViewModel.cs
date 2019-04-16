using Angelo.Common.Models;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserMembershipViewModel
    {
        public string UserGroupId { get; set; }
        public string UserGroupName { get; set; }
        public bool AllowPublicEnrollment { get; set; }
        public bool IsMember { get; set; }
        public bool AllowEmailMessaging { get; set; }
        public bool AllowSmsMessaging { get; set; }
    }
}
