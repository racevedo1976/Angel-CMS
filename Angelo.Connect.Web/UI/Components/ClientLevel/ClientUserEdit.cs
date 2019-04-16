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
    public class ClientUserEdit : ViewComponent
    {
        private UserManager _userManager;

        public ClientUserEdit(UserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId, string userId)
        {
            // TODO: Ensure directory is valid for client
            var user = await _userManager.GetUserAsync(userId);
            var model = user.ProjectTo<UserViewModel>();

            var providers = await _userManager.GetWirelessProvidersAsync();
            var providerSelectList = providers.Select(p => new SelectListItem() { Value = p.Id, Text = p.Name }).ToList();

            providerSelectList.Insert(0, new SelectListItem() { Value = "", Text = "----- Select -----" });
            ViewData["providerSelectList"] = providerSelectList;

            return await Task.Run(() => View(model));
        }
    }
}
