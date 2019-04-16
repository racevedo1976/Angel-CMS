using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PoolRoleClaimViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Permission Scope", ShortName = "Scope")]
        [Required(ErrorMessage = "The permission key is required")]
        public string TypeName { get; set; }

        [Display(Name = "Permission Key", ShortName = "Permission")]
        [StringLength(maximumLength: 100, ErrorMessage = "The permission scope value cannot exceed {1} characters")]
        [Required(ErrorMessage = "The permission scope value is required")]
        public string Value { get; set; }

        public string PoolId { get; set; }
        public string RoleId { get; set; }
    }
}
