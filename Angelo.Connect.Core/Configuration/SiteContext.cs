using System;
using System.Collections.Generic;

using Angelo.Connect.Models;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Configuration
{
    public class SiteContext
    {      
        public SiteContext()
        {
            ProductContext = new ProductContext();
            Cultures = new List<SiteCulture>();
        }

        public string SiteId { get; set; }
        public string SiteTitle { get; set; }
        public string SiteBanner { get; set; }
        public string TenantKey { get; set; }
        public string SecurityPoolId { get; set; }    
        public string DefaultDomain { get; set; }
        public string DefaultUrl { get; set; }
        public bool Published { get; set; }
        public Enum SiteSearchProvider { get; set; }

        public Client Client { get; set; }
        public SiteTemplate Template { get; set; }

        public SiteTemplateTheme Theme { get; set; }

        public ProductContext ProductContext { get; set; }
        public IEnumerable<SiteCulture> Cultures { get; set; }

        public string AuthCookieName { get; set; }

        public Dictionary<string, string> Options { get; set; }
        public IEnumerable<SiteSetting> SiteSettings { get; set; }
    }
}
