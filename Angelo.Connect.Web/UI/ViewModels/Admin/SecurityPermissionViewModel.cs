using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SecurityPermissionViewModel
    {

        public SecurityPermissionViewModel()
        {
            Permissions = new List<SecurityPermissionViewModel>();
            
        }

        public string Title { get; set; }
        public bool Selected { get; set; }
        public IList<SecurityPermissionViewModel> Permissions { get; set; }


    }
}
