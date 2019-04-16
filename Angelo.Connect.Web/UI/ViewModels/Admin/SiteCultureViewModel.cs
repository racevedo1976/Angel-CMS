using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCultureViewModel
    {
        public SiteCultureViewModel()
        {
            IsSelected = true;
        }

        [Display(Name = "Site Id", ShortName = "Id")]
        public string SiteId { get; set; }

        [Display(Name = "Site Culture Key", ShortName = "Culture")]
        public string CultureKey { get; set; }

        [Display(Name = "Display Name", ShortName = "Display Name")]
        public string DisplayName { get; set; }

        public bool IsSelected { get; set; }
    }
}
