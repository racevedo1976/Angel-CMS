using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Common.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SitesAdminViewModel
    {
        public PagedResult<SiteViewModel> SitePages { get; set; }
        public SiteViewModel SelectedSite { get; set; }

        public SitesAdminViewModel()
        {
            SitePages = new PagedResult<SiteViewModel>();
            SelectedSite = new SiteViewModel();
        }

        public SiteViewModel FindSiteId(string siteId)
        {
            if (siteId == null) return null;
            foreach(var site in SitePages.Data)
            {
                if (site.Id == siteId)
                    return site;
            }
            return null;
        }


    }
}
