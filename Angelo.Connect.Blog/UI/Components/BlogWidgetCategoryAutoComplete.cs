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
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.UI.ViewModels;


namespace Angelo.Connect.Blog.UI.Components
{
    public class BlogWidgetCategoryAutoComplete : ViewComponent
    {
        private CategoryManager _categoryManager;
        private BlogDbContext _blogDb;
        private BlogWidgetService _blogWidgetService;
        private BlogManager _blogManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogWidgetCategoryAutoComplete
        (
            CategoryManager categoryManager, 
            BlogDbContext blogDb,
            BlogManager blogManager,
            BlogWidgetService blogWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _categoryManager = categoryManager;
            _blogDb = blogDb;
            _blogWidgetService = blogWidgetService;
            _blogManager = blogManager;

            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, bool shared = false)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new BlogWidgetCategoryFormViewModel
            {
                WidgetId = widgetId,
                UserId = userContext.UserId
            };

            if (!shared)
            {
                viewModel.BlogCategories = _blogManager.GetBlogCategoriesOwnedByUser(userContext.UserId);
            }
            else
            {
                viewModel.BlogCategories = _blogManager.GetBlogCategoriesSharedWithUser(userContext);
            }
           

            return View("/UI/Views/Components/BlogWidgetCategoryAutoComplete.cshtml", viewModel);
        }
    }
}
