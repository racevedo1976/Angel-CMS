using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SecurityRolePermissionsViewModel
    {
        public SecurityRolePermissionsViewModel()
        {

        }

        public string RoleId { get; set; }
        public string PoolId { get; set; }
        public List<string> SelectedPermissionGroups { get; set; }
    }
}
