using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Configuration;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.Security;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarWidgetGroups : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;
        private IContextAccessor<UserContext> _userContextAccessor;
        private SiteContext _siteContext;
        CalendarSecurityService _calendarSecurity;

        public CalendarWidgetGroups(CalendarQueryService calendarQueryService, IContextAccessor<UserContext> userContextAccessor, SiteContext siteContext, CalendarSecurityService calendarSecurity)
        {
            _calendarQueryService = calendarQueryService;
            _userContextAccessor = userContextAccessor;
            _siteContext = siteContext;
            _calendarSecurity = calendarSecurity;
        }

        public async Task<IViewComponentResult> InvokeAsync(string calendarWidgetId, bool allEventGroups = false)
        {
            var userContext = _userContextAccessor.GetContext();

            var allGroups = !allEventGroups
                               ? _calendarQueryService.GetEventGroupsByUserId(userContext.UserId)
                               : _calendarQueryService.GetSharedEventGroups(_calendarSecurity.GetEventGroupsSharedWithMe(), userContext.UserId);

            var selectedWigetEventGroups = _calendarQueryService.GetWidgetGroups(calendarWidgetId).ToList();

            ViewData["EventGroups"] = allGroups;
            ViewData["SelectedWidgetEventGroups"] = selectedWigetEventGroups;

            ViewData["AllEventGroups"] = allEventGroups;

            ViewData["widgetId"] = calendarWidgetId;
            ViewData["siteId"] = _siteContext.SiteId;
            
            return View();
        }
    }
}
