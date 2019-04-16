using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class Tag
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string TagName { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
