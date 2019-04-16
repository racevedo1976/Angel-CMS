using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.UI.ViewModels;
using Angelo.Connect.Security;
using Angelo.Connect.Blog.Services;

namespace Angelo.Connect.Blog.UI.Components
{
    public class BlogWidgetCategoryForm : ViewComponent
    {
        private BlogDbContext _blogDb;
        private BlogWidgetService _blogWidgetService;
        private BlogManager _blogManager;
        private CategoryManager _categoryManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogWidgetCategoryForm
        (
            CategoryManager categoryManager, 
            BlogDbContext blogDb, 
            BlogManager blogManager,
            BlogWidgetService blogWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _blogDb = blogDb;
            _blogWidgetService = blogWidgetService;
            _blogManager = blogManager;
            _categoryManager = categoryManager;

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

            var categories = new List<BlogCategory>();
           
            if (!shared)
            {
                viewModel.BlogCategories = _blogManager.GetBlogCategoriesOwnedByUser(userContext.UserId);
            }
            else
            { 
                viewModel.BlogCategories = _blogManager.GetBlogCategoriesSharedWithUser(userContext);
            }

            viewModel.SelectedCategoryIds = await GetSelectedCategories(widgetId);

            return View("/UI/Views/Components/BlogWidgetCategoryForm.cshtml", viewModel);
        }

        public async Task<IEnumerable<string>> GetSelectedCategories(string widgetId)
        {
            var selectedCategories = await _blogDb
                .BlogWidgetCategories
                .Where(x => x.WidgetId == widgetId)
                .ToListAsync();

            return selectedCategories.Select(x => x.CategoryId);
        }
    }
}
