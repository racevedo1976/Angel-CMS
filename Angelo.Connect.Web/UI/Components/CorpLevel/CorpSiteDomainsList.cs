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

namespace Angelo.Connect.Web.UI.Components
{
    public class CorpSiteDomainsList : ViewComponent
    {
        private SiteManager _siteManager;

        public CorpSiteDomainsList(SiteManager siteManager)
        {
            _siteManager = siteManager;
        }

        protected async Task<List<CorpSiteDomainViewModel>> GetSiteDomainsAsync(string siteId)
        {
            var domains = await _siteManager.GetDomainsAsync(siteId);
            if (domains == null)
                return new List<CorpSiteDomainViewModel>();
            var model = domains.ProjectTo<CorpSiteDomainViewModel>();
            return model.ToList();
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "")
        {
            if (string.IsNullOrEmpty(siteId))
                return new ViewComponentPlaceholder();
            var model = await GetSiteDomainsAsync(siteId);
            ViewData["siteId"] = siteId;
            return View(model);
        }

    }
}

