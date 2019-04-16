using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Security;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteNotificationList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string ownerLevel, string ownerId)
        {
            ViewData["ownerLevel"] = ownerLevel;
            ViewData["ownerId"] = ownerId ?? string.Empty;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
