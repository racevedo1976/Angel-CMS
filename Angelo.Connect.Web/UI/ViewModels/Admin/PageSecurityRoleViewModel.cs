using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PageSecurityRoleViewModel
    {
        public string PageId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public String AccessLevel { get; set; }

        public string Id
        {
            get
            {
                return string.Concat("P:", PageId, "+R:", RoleId);
            }
        }
    }
}
