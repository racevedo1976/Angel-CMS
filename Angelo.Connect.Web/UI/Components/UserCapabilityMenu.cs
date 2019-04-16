using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;


namespace Angelo.Connect.Web.UI.Components
{
    public class UserCapabilityMenu : ViewComponent
    {
        private MenuProvider _menuProvider;

        public UserCapabilityMenu(MenuProvider menuProvider)
        {
            _menuProvider = menuProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _menuProvider.GetMenuItemsAsync(MenuType.AdminTools);

            return await Task.Run(() => {
                return View(model);
            });
        }
    }
}
