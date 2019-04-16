using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class PageMaster
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string SiteId { get; set; }
        public string TemplateId { get; set; }
        public string PreviewPath { get; set; }
        public bool IsDefault { get; set; }
        public bool IsSystemPage { get; set; }

        public ICollection<Page> DerivedPages { get; set; }
        public Site Site { get; set; }
    }
}
