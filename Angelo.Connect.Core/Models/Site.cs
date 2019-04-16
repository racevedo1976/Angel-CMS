using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Rendering;

namespace Angelo.Connect.Models
{
    public class Site
    {
        public Site()
        {
            Domains = new List<SiteDomain>();
            Cultures = new List<SiteCulture>();
            SiteCollectionMaps = new List<SiteCollectionMap>();
            Pages = new List<Page>();
            SiteSettings = new List<SiteSetting>();
            SiteDirectories = new List<SiteDirectory>();

            //MDJ: Upon site creation, this value will not be known yet since themes vary for site templates
            //     However, all site templates must have a "default" theme.
            ThemeId = "default";
        }

        public string Id { get; set; }
        public string ClientId { get; set; }
        public string TenantKey { get; set; }
        public string SecurityPoolId { get; set; }
        public string ClientProductAppId { get; set; }
        public string Title { get; set; }
        public string Banner { get; set; }
        public string SiteTemplateId { get; set; }
        public string ThemeId { get; set; }
        public bool Published { get; set; }
        public string DefaultCultureKey { get; set; }


        public Client Client { get; set; }
        public ClientProductApp ClientProductApp { get; set; }
        public SiteTemplate SiteTemplate { get; set; }

        public ICollection<SiteCulture> Cultures { get; set; }
        public ICollection<SiteDomain> Domains { get; set; }
        public ICollection<SiteCollectionMap> SiteCollectionMaps { get; set; }
        public ICollection<Page> Pages { get; set; }
        public ICollection<PageMaster> PageMasters { get; set; }

        public ICollection<NavigationMenu> NavigationMenus { get; set; }
        public ICollection<SiteSetting> SiteSettings { get; set; }
        public ICollection<SiteDirectory> SiteDirectories { get; set; }
    }
}