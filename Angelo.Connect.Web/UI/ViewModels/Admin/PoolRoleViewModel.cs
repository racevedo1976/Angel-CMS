using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PoolRoleViewModel : RoleViewModel
    {
        [Display(Name = "Pool Id", ShortName = "Pool Id")]
        public string PoolId { get; set; }
    }
}
