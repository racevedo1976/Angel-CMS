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
    public class CategoryEditForm : ViewComponent
    {
        private CategoryManager _categoryManager;

        public CategoryEditForm(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = "", string siteId = "", string categoryId = "")
        {
            ViewData["ClientId"] = clientId;
            ViewData["SiteId"] = siteId;
            ViewData["CategoryId"] = categoryId;
            var model = new CategoryViewModel();

            if (!string.IsNullOrEmpty(categoryId))
            {
                var currentCategory = await _categoryManager.GetClientCategoryByIdAsync(categoryId);
                model = currentCategory.ProjectTo<CategoryViewModel>();
            }

            return View(model);
        }
    }
}
