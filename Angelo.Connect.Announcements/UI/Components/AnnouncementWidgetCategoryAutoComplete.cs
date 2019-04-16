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
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.UI.ViewModels;


namespace Angelo.Connect.Announcement.UI.Components
{
    public class AnnouncementWidgetCategoryAutoComplete : ViewComponent
    {
        private CategoryManager _categoryManager;
        private AnnouncementDbContext _announcementDb;
        private AnnouncementWidgetService _announcementWidgetService;
        private AnnouncementManager _announcementManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementWidgetCategoryAutoComplete
        (
            CategoryManager categoryManager, 
            AnnouncementDbContext announcementDb,
            AnnouncementManager announcementManager,
            AnnouncementWidgetService announcementWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _categoryManager = categoryManager;
            _announcementDb = announcementDb;
            _announcementWidgetService = announcementWidgetService;
            _announcementManager = announcementManager;

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

            if (!shared)
            {
                viewModel.AnnouncementCategories = _announcementManager.GetAnnouncementCategoriesOwnedByUser(userContext.UserId);
            }
            else
            {
                viewModel.AnnouncementCategories = _announcementManager.GetAnnouncementCategoriesSharedWithUser(userContext);
            }
           

            return View("/UI/Views/Components/AnnouncementWidgetCategoryAutoComplete.cshtml", viewModel);
        }
    }
}
