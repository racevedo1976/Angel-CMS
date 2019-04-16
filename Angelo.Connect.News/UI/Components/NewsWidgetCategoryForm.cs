using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.UI.ViewModels;
using Angelo.Connect.Security;
using Angelo.Connect.News.Services;

namespace Angelo.Connect.News.UI.Components
{
    public class NewsWidgetCategoryForm : ViewComponent
    {
        private NewsDbContext _NewsDb;
        private NewsWidgetService _NewsWidgetService;
        private NewsManager _NewsManager;
        private CategoryManager _categoryManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsWidgetCategoryForm
        (
            CategoryManager categoryManager, 
            NewsDbContext NewsDb, 
            NewsManager NewsManager,
            NewsWidgetService NewsWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _NewsDb = NewsDb;
            _NewsWidgetService = NewsWidgetService;
            _NewsManager = NewsManager;
            _categoryManager = categoryManager;

            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, bool shared = false)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new NewsWidgetCategoryFormViewModel
            {
                WidgetId = widgetId,
                UserId = userContext.UserId
            };

            var categories = new List<NewsCategory>();
           
            if (!shared)
            {
                viewModel.NewsCategories = _NewsManager.GetNewsCategoriesOwnedByUser(userContext.UserId);
            }
            else
            { 
                viewModel.NewsCategories = _NewsManager.GetNewsCategoriesSharedWithUser(userContext);
            }

            viewModel.SelectedCategoryIds = await GetSelectedCategories(widgetId);

            return View("/UI/Views/Components/NewsWidgetCategoryForm.cshtml", viewModel);
        }

        public async Task<IEnumerable<string>> GetSelectedCategories(string widgetId)
        {
            var selectedCategories = await _NewsDb
                .NewsWidgetCategories
                .Where(x => x.WidgetId == widgetId)
                .ToListAsync();

            return selectedCategories.Select(x => x.CategoryId);
        }
    }
}
