using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteUserRolesEdit : ViewComponent
    {
        private SiteManager _siteManager;
        private UserManager _userManager;
        private SecurityPoolManager _poolManager;

        public SiteUserRolesEdit(SiteManager siteManager, UserManager userManager, SecurityPoolManager poolManager)
        {
            _userManager = userManager;
            _poolManager = poolManager;
            _siteManager = siteManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId, string userId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            var allRoles = await _poolManager.GetRolesAsync(site.SecurityPoolId);
            var userRoles = await _poolManager.GetUserRolesAsync(site.SecurityPoolId, userId);

            // TODO - Ensure user's directory is mapped to this site's security pool

            var model = allRoles.Select(x => new UserRoleViewModel
            {
                RoleId = x.Id,
                RoleName = x.Name,
                Selected = userRoles.Any(y => y.Id == x.Id)
            }).ToList();

            ViewData["UserId"] = userId;

            return View(model);
        }
    }
}
