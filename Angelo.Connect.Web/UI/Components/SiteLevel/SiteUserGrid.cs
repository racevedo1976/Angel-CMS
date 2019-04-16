using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;


namespace Angelo.Connect.Web.UI.Components.SiteLevel
{
    public class SiteUserGrid : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string siteId, string directoryId)
        {
            ViewData["SiteId"] = siteId;
            ViewData["DirectoryId"] = directoryId;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
