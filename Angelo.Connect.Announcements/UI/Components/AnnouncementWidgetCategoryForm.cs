using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.UI.ViewModels;
using Angelo.Connect.Security;
using Angelo.Connect.Announcement.Services;

namespace Angelo.Connect.Announcement.UI.Components
{
    public class AnnouncementWidgetCategoryForm : ViewComponent
    {
        private AnnouncementDbContext _announcementDb;
        private AnnouncementWidgetService _announcementWidgetService;
        private AnnouncementManager _announcementManager;
        private CategoryManager _categoryManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementWidgetCategoryForm
        (
            CategoryManager categoryManager, 
            AnnouncementDbContext announcementDb, 
            AnnouncementManager announcementManager,
            AnnouncementWidgetService announcementWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _announcementDb = announcementDb;
            _announcementWidgetService = announcementWidgetService;
            _announcementManager = announcementManager;
            _categoryManager = categoryManager;

            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, bool shared = false)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new AnnouncementWidgetCategoryFormViewModel
            {
                WidgetId = widgetId,
                UserId = userContext.UserId
            };

            var categories = new List<AnnouncementCategory>();
           
            if (!shared)
            {
                viewModel.AnnouncementCategories = _announcementManager.GetAnnouncementCategoriesOwnedByUser(userContext.UserId);
            }
            else
            { 
                viewModel.AnnouncementCategories = _announcementManager.GetAnnouncementCategoriesSharedWithUser(userContext);
            }

            viewModel.SelectedCategoryIds = await GetSelectedCategories(widgetId);

            return View("/UI/Views/Components/AnnouncementWidgetCategoryForm.cshtml", viewModel);
        }

        public async Task<IEnumerable<string>> GetSelectedCategories(string widgetId)
        {
            var selectedCategories = await _announcementDb
                .AnnouncementWidgetCategories
                .Where(x => x.WidgetId == widgetId)
                .ToListAsync();

            return selectedCategories.Select(x => x.CategoryId);
        }
    }
}
