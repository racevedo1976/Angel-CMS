using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class CategoryDataController : Controller
    {
        private CategoryManager _categoryManager;

        public CategoryDataController(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [Authorize]
        [HttpPost, Route("/api/category/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string level, string clientId, string siteId)
        {
            if (level == "Client")
            {
                var categories = await _categoryManager.GetClientCategoriesAsync();
                var model = categories.ProjectTo<CategoryViewModel>();
                return Json(model.ToDataSourceResult(request));
            }
            else if (level == "Site")
            {
                var categories = await _categoryManager.GetSiteCategoriesAsync(siteId);
                var model = categories.ProjectTo<CategoryViewModel>();
                return Json(model.ToDataSourceResult(request));
            }
            else
            {
                var model = new List<CategoryViewModel>();
                return Json(model.ToDataSourceResult(request));
            }
            
        }

        [Authorize]
        [HttpPost, Route("/api/category/dataSite")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string siteId)
        {
            var categories = await _categoryManager.GetSiteCategoriesAsync(siteId);
            var model = categories.ProjectTo<CategoryViewModel>();
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpPost, Route("/api/category")]
        public async Task<JsonResult> Create(CategoryViewModel vm)
        {
            var category = new Category();
            var newId = "";
            category.OwnerId = vm.OwnerId;
            category.Title = vm.Title;
            category.OwnerLevel = vm.OwnerLevel;

            newId = await _categoryManager.CreateCategoryAsync(category);
            category.Id = newId;

            return Json(category);
        }

        [Authorize]
        [HttpDelete, Route("/api/category")]
        public async Task<JsonResult> Delete([DataSourceRequest]DataSourceRequest request, CategoryViewModel vm)
        {
            var categoryId = vm.Id;
            var result = await _categoryManager.DeleteCategoryAsync(categoryId);
            return Json(Ok());
        }

        [Authorize]
        [HttpPut, Route("/api/category")]
        public async Task<JsonResult> Edit(CategoryViewModel vm)
        {
            var category = new Category();
            category.Id = vm.Id;
            category.Title = vm.Title;

            var isUpdated = await _categoryManager.UpdateCategoryAsync(category);

            return Json(category);
        }

        [Authorize]
        [HttpPut, Route("/api/filterMenuType")]
        public async Task<JsonResult> FilterMenuTypes([DataSourceRequest]DataSourceRequest request)
        {
            var categories = await _categoryManager.GetFilterMenuTypesAsync();
            var model = categories.ProjectTo<CategoryViewModel>();
            return Json(model.ToDataSourceResult(request));
        }

    }
}
