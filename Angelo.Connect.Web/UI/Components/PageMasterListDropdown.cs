using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Components
{
    public class PageMasterListDropdown : ViewComponent
    {
        private PageMasterManager _pageMasterManager;

        public PageMasterListDropdown(PageMasterManager pageMasterManager)
        {
            _pageMasterManager = pageMasterManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId)
        {
            ViewData["SiteId"] = siteId;
            return View();
        }
    }
}
