using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Rendering;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteTemplateViewModel
    {
        public string SiteId { get; set; }
        public IEnumerable<SiteTemplate> AvailableTemplates { get; set; }
        public SiteTemplate ActiveTemplate { get; set; }
        public SiteTemplate SelectedTemplate { get; set; }
        public SiteTemplateTheme SelectedTheme { get; set; }     
    }
}
