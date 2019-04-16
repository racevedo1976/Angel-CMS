using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Models
{
    public enum ContentStatus
    {
        [Display(Name = "Scratch")]
        Scratch = 0,

        [Display(Name = "Draft")]
        Draft = 1,

        [Display(Name = "Published")]
        Published = 2,

        [Display(Name = "Archived")]
        Archived = 3,

        [Display(Name = "Removed")]
        Deleted = 4
    }
}
