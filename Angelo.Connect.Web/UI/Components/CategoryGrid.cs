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
    public class CategoryGrid : ViewComponent
    {
        private CategoryManager _categoryManager;

        public CategoryGrid(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId, string siteId, string level)
        {
            ViewData["ClientId"] = clientId;
            ViewData["SiteId"] = siteId;
            ViewData["Level"] = level;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
