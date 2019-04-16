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
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteRoleEditViewComponent : ViewComponent
    {
        private SiteManager _siteManager;
        private RoleManager _roleManager;

        public SiteRoleEditViewComponent(SiteManager siteManager, RoleManager roleManager)
        {
            _siteManager = siteManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "", string roleId = "")
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            var role = await _roleManager.GetByIdAsync(roleId);

            var model = new PoolRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                IsDefault = role.IsDefault,
                IsLocked = role.IsLocked,
                PoolId = site.SecurityPoolId
            };

            ViewData["siteId"] = siteId;

            return View(model);
        }
    }
}
