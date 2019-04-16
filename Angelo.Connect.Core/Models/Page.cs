using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class Page
    {
        public Page()
        {
            Type = PageType.Normal;
            ChildPages = new List<Page>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }     
        public string Keywords { get; set; } // Search Meta
        public string Summary { get; set; }  // Search Meta
        public PageType Type { get; set; }
        public string Layout { get; set; }
        public string SiteId { get; set; }
        public string ParentPageId { get; set; }
        public string PageMasterId { get; set; }
        public bool IsPrivate { get; set; }
        public string UserId { get; set; }
        public bool IsHomePage { get; set; }

        public Site Site { get; set; }
        public Page ParentPage { get; set; }
        public PageMaster MasterPage { get; set; }

        public string PublishedVersionCode { get; set; }
        public string InitialSeedStrategy { get; set; }

        public ICollection<Page> ChildPages { get; set; }
    }
}
