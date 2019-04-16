using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Services;

namespace Angelo.Connect.Configuration
{
    public class SiteAdminContext
    {
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private SiteDomain _defaultDomain;

        public SiteAdminContext(Site site, SiteManager siteManager, ClientManager clientManager)
        {
            _siteManager = siteManager;
            _clientManager = clientManager;

            // Initialize
            Site = site;
            Product = _clientManager.GetProductContextAsync(site.ClientProductAppId).Result;
        }

        public Site Site { get; private set; }

        public ProductContext Product { get; private set; }
  
    }
}
