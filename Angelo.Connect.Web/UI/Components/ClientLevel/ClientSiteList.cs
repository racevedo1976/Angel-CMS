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

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientSiteListViewComponent : ViewComponent
    {
        public ClientSiteListViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string appId)
        {
            ViewData["appId"] = appId;
            return View();
        }

    }
}
