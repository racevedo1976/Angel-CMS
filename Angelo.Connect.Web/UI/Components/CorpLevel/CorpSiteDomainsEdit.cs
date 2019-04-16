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
    public class CorpSiteDomainsEdit : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string siteId = "", string domainKey = "", bool isDefault = false)
        {
            if (string.IsNullOrEmpty(siteId))
                return new ViewComponentPlaceholder();

            var model = new CorpSiteDomainViewModel();

            model.SiteId = siteId;
            model.OriginalDomainKey = domainKey;
            model.DomainKey = domainKey;
            model.IsDefault = isDefault;

            return View(model);
        }
    }
}

