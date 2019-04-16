using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class SiteTemplateNav
    {
        public string Title { get; set; }
        public IEnumerable<SiteTemplateNavItem> Items { get; set; }
    }

    public class SiteTemplateNavItem
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
    }
}
