using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class CorpSiteDomainViewModel
    {
        [Display(Name = "Site Id", ShortName = "Id")]
        public string SiteId { get; set; }

        [Display(Name = "Site Domain Key", ShortName = "Domain")]
        public string DomainKey { get; set; }

        [Display(Name = "Is Default", ShortName = "Default")]
        public bool IsDefault { get; set; }

        public string OriginalDomainKey { get; set; }
    }
}
