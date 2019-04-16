using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components.ClientLevel
{
    public class ClientSiteDomains : ViewComponent
    {
        private SiteManager _siteManager;
        public ClientSiteDomains(SiteManager siteManager )
        {
            _siteManager = siteManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "")
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            if (site == null)
                return new ViewComponentPlaceholder();
            var model = site.ProjectTo<SiteViewModel>();

            return View(model);
        }


    }
}
