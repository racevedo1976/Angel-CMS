using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Rendering;
using Angelo.Connect.Models;

namespace Angelo.Connect.UI.ViewModels
{
    public class MasterPageViewModel
    {
        public string MasterPageId { get; set; }
        public string MasterPageViewPath { get; set; }
        public string MasterPageTreeId { get; set; }

        public string PageType { get; set; }
        public string PageId { get; set; }
        public string PageViewPath { get; set; }
        public object PageModel { get; set; }
        public string PageTreeId { get; set; }

        public string PageVersionCode { get; set; }

        public ContentStatus PageStatus { get; set; }

        public bool LoadDesignTools { get; set; }
    }
}
