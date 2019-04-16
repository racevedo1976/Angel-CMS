using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class SiteCollectionMap
    {
        public SiteCollectionMap()
        {
            SiteCollection = new SiteCollection();
            Site = new Site();
        }

        public string SiteCollectionId { get; set; }
        public string SiteId { get; set; }

        public SiteCollection SiteCollection { get; set; }
        public Site Site { get; set; }
    }
}

