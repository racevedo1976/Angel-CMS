using Angelo.Common.Models;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserGroupMembershipViewModel
    {
        public string UserGroupId { get; set; }
        public string UserGroupName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AccessLevel AccessLevel { get; set; }
        public string AccessLevelName { get; set; }
        public bool AllowEmailMessaging { get; set; }
        public bool AllowSmsMessaging { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }

        public string Id
        {
            get { return UserGroupId + ":" + UserId; }
        }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }

    }
}
