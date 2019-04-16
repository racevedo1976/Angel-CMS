using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class NavigationMenuAddForm : ViewComponent
    {
        private NavigationMenuManager _navigationMenuManager;

        public NavigationMenuAddForm(NavigationMenuManager navigationMenuManager)
        {
            _navigationMenuManager = navigationMenuManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "")
        {
            if (string.IsNullOrEmpty(siteId))
                return new ViewComponentPlaceholder();
            else
            {
                var model = new NavigationMenuViewModel();
                model.Id = string.Empty;
                model.SiteId = siteId;
                model.Title = string.Empty;

                ViewData["FormTitle"] = "Create new Navigation Menu:";

                return View(model);
            }
        }
    }
}
