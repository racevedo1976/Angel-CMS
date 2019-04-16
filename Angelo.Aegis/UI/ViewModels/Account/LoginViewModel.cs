using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.UI.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username.Name", ShortName = "Username.ShortName")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password.Name", ShortName = "Password.ShortName")]
        public string Password { get; set; }

        [Display(Name = "RememberMe.Name", ShortName = "RememberMe.ShortName")]
        public bool RememberMe { get; set; }
    }
}
