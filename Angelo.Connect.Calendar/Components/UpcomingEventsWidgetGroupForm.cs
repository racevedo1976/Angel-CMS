using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Calendar.Data;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.UI.ViewModels;
using Angelo.Connect.Security;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Calendar.Security;

namespace Angelo.Connect.Calendar.UI.Components
{
    public class UpcomingEventsWidgetGroupForm : ViewComponent
    {
        private CalendarDbContext _calendarDb;
        private CalendarWidgetService _calendarWidgetService;
        private CalendarQueryService _calendarQueryService;
        private CalendarSecurityService _calendarSecurity;
        private CategoryManager _categoryManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public UpcomingEventsWidgetGroupForm
        (
            CategoryManager categoryManager, 
            CalendarDbContext blogDb, 
            CalendarQueryService calendarQueryService,
            CalendarWidgetService calendarWidgetService,
            CalendarSecurityService calendarSecurity,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _calendarDb = blogDb;
            _calendarWidgetService = calendarWidgetService;
            _calendarQueryService = calendarQueryService;
            _calendarSecurity = calendarSecurity;
            _categoryManager = categoryManager;

            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, bool shared = false)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new UpcomingEventsGroupFormViewModel
            {
                WidgetId = widgetId,
                UserId = userContext.UserId
            };
           
            if (!shared)
            {
                viewModel.UpcomingEventGroups = _calendarQueryService.GetEventGroupsByUserId(userContext.UserId);
            }
            else
            { 
                viewModel.UpcomingEventGroups = _calendarSecurity.GetEventGroupsSharedWithUser(userContext);
            }

            viewModel.SelectedGroupIds = await GetSelectedCategories(widgetId);

            return View(viewModel);
        }

        public async Task<IEnumerable<string>> GetSelectedCategories(string widgetId)
        {
            var selectedCategories = await _calendarDb.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == widgetId).ToListAsync();

            return selectedCategories.Select(x => x.EventGroupId);
        }
    }
}
