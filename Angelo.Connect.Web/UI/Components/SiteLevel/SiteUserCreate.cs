using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Common.Mvc.ActionResults;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteUserCreate : ViewComponent
    {
        private UserManager _userManager;

        public SiteUserCreate(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId, string directoryId)
        {
            // TODO: Ensure directory is valid for site
            UserViewModel model = new UserViewModel();
            model.DirectoryId = directoryId;
            var providers = await _userManager.GetWirelessProvidersAsync();
            var providerSelectList = providers.Select(p => new SelectListItem() { Value = p.Id, Text = p.Name }).ToList();

            providerSelectList.Insert(0, new SelectListItem() { Value = "", Text = "----- Select -----" });
            ViewData["providerSelectList"] = providerSelectList;

            // NOTE: The Create Component uses the same form as the Edit Component
            return await Task.FromResult(View("SiteUserEdit", model));
        }
    }
}
