using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteDirectoryViewModel
    {
        public SiteDirectoryViewModel()
        {
            Selected = true;
        }
        public string DirectoryId { get; set; }
        public string Name { get; set; }
        public string SiteId { get; set; }
        public bool Selected { get; set; }
    }
}
