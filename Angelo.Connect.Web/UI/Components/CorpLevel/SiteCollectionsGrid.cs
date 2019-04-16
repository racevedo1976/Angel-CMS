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
    public class SiteCollectionsGrid : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            ViewData["ClientId"] = clientId;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
