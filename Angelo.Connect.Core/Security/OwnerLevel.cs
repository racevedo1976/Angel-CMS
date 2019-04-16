using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Security
{
    public enum OwnerLevel
    {
        [Display(Name = "Global")]
        Global = 1,

        [Display(Name = "Client")]
        Client = 3,

        [Display(Name = "Site")]
        Site = 5,

        [Display(Name = "Role")]
        Role = 7,

        [Display(Name = "User")]
        User = 9
    }
}
