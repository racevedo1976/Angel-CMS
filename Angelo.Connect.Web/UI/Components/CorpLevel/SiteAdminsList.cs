using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Html;
using Angelo.Common.Mvc.ActionResults;
//using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteAdminsList : ViewComponent
    {
        private SiteManager _siteManager;

        public SiteAdminsList(SiteManager siteManager)
        {
            _siteManager = siteManager;
        }

        protected async Task<string> GetPoolId(string siteId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            if (site == null)
                return string.Empty;
            else
                return site.SecurityPoolId;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "")
        {
            return new ViewComponentPlaceholder();


            //if (siteId == null)
            //    return View();
            //ViewData["siteId"] = siteId;
            //ViewData["poolId"] = await GetPoolId(siteId);
            //return View("SiteAdminsList");
        }

    }
}

