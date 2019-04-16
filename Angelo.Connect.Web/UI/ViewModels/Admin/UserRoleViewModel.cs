using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
