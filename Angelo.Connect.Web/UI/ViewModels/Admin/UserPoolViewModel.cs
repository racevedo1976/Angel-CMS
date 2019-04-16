using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserPoolViewModel
    {
        [Display(Name = "Pool Id")]
        public string PoolId { get; set; }

        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
