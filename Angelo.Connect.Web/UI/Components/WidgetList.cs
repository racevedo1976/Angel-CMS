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
    public class WidgetList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string siteId)
        {
            ViewData["siteId"] = siteId;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
