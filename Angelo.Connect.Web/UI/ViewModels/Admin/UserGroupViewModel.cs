using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserGroupViewModel
    {
        public UserGroupViewModel()
        {
            Memberships = new List<UserGroupMembership>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public UserGroupType UserGroupType { get; set; }
        public bool AllowPublicEnrollment { get; set; }

        public List<UserGroupMembership> Memberships { get; set; }

    }
}

    