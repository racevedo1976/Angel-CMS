using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteSettingsViewModel
    {
        public SiteSettingsViewModel()
        {
        }


        //[Display(Name = "Custom 404 Page", ShortName = "404 Page")]
        //public string Custom404 { get; set; }

        //[Display(Name = "Custom 401 Page", ShortName = "401 Page")]
        //public string Custom401 { get; set; }

        //[Display(Name = "Custom 500 Page", ShortName = "500 Page")]
        //public string Custom500 { get; set; }

        //[Display(Name = "Custom Landing Page", ShortName = "Landing Page")]
        //public string LandingPage { get; set; }

        [Display(Name = "Google Tracking Code", ShortName = "Tracking Id")]
        public string GoogleTrackingId { get; set; }

     


    }
}
