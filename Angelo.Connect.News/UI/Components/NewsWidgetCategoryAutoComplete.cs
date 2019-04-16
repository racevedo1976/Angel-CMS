using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.UI.ViewModels;


namespace Angelo.Connect.News.UI.Components
{
    public class NewsWidgetCategoryAutoComplete : ViewComponent
    {
        private CategoryManager _categoryManager;
        private NewsDbContext _NewsDb;
        private NewsWidgetService _NewsWidgetService;
        private NewsManager _NewsManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsWidgetCategoryAutoComplete
        (
            CategoryManager categoryManager, 
            NewsDbContext NewsDb,
            NewsManager NewsManager,
            NewsWidgetService NewsWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _categoryManager = categoryManager;
            _NewsDb = NewsDb;
            _NewsWidgetService = NewsWidgetService;
            _NewsManager = NewsManager;

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

            if (!shared)
            {
                viewModel.NewsCategories = _NewsManager.GetNewsCategoriesOwnedByUser(userContext.UserId);
            }
            else
            {
                viewModel.NewsCategories = _NewsManager.GetNewsCategoriesSharedWithUser(userContext);
            }
           

            return View("/UI/Views/Components/NewsWidgetCategoryAutoComplete.cshtml", viewModel);
        }
    }
}
