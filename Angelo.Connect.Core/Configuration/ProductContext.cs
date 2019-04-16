using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Configuration
{
    public class ProductContext
    {
        public ProductContext()
        {
            Features = new List<Feature>();
            SiteTemplates = new List<SiteTemplate>();
        }

        public string AppId { get; set; }
        public string AppTitle { get; set; }
        public int MaxSiteCount { get; set; }
        public int ActiveSiteCount { get; set; }
        public int BaseClientMB { get; set; }
        public int PerSiteMB { get; set; }
        public Client Client { get; set; }
        public Product Product { get; set; }
        public DateTime SubscriptionStartUTC { get; set; }
        public DateTime? SubscriptionEndUTC { get; set; }
        public ICollection<Feature> Features { get; set; }
        public ICollection<SiteTemplate> SiteTemplates { get; set; }

        public bool IsActive
        {
            get
            {
                var now = DateTime.UtcNow;
                return ((SubscriptionStartUTC < now) &&
                        ((SubscriptionEndUTC == null) || (SubscriptionEndUTC > now)));
            }
        }

        public int TotalMB
        {
            get
            {
                return BaseClientMB + (MaxSiteCount * PerSiteMB);
            }
        }

    }
}
