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
using Angelo.Common.Mvc.ActionResults;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteRoleCreateViewComponent : ViewComponent
    {
        private SiteManager _siteManager;

        public SiteRoleCreateViewComponent(SiteManager siteManager)
        {
            _siteManager = siteManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);

            var model = new PoolRoleViewModel()
            {
                PoolId = site.SecurityPoolId,
                IsDefault = false,
                IsLocked = false
            };

            ViewData["siteId"] = siteId;

            return View(model);
        }


    }
}
