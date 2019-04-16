using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class RoleViewModel
    {

        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Name", ShortName = "Name")]
        [StringLength(maximumLength: 30, ErrorMessage = "Name.Error.MaxLength" )]
        [Required(ErrorMessage = "Name.Error.Required")]
        public string Name { get; set; }

        [Display(Name = "Default", ShortName = "Default")]
        public bool IsDefault { get; set; }

        [Display(Name = "Locked", ShortName = "Locked")]
        public bool IsLocked { get; set; }
    }
}
