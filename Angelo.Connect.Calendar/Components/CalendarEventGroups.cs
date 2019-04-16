using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Security;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Security;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarEventGroups : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;
        private CalendarSecurityService _calendarSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public CalendarEventGroups(CalendarQueryService calendarQueryService, IContextAccessor<UserContext> userContextAccessor, CalendarSecurityService calendarSecurity)
        {
            _calendarQueryService = calendarQueryService;
            _userContextAccessor = userContextAccessor;
            _calendarSecurity = calendarSecurity;


        }
        public async Task<IViewComponentResult> InvokeAsync(string id, bool allEventGroups = false)
        {
            var userContext = _userContextAccessor.GetContext();
            var eventGroups = new List<CalendarEventGroup>();

            //Get Event or set default
            ViewData["eventId"] = id;

            var theEvent = _calendarQueryService.GetEventById(id);

            var allMyGroups = !allEventGroups
                                ? _calendarQueryService.GetEventGroupsByUserId(userContext.UserId)
                                : _calendarQueryService.GetSharedEventGroups(_calendarSecurity.GetEventGroupsSharedWithMe(), userContext.UserId);
            


            //TODO add my groups or option to add all other groups
            ViewData["EventGroups"] = allMyGroups;
            ViewData["AllEventGroups"] = allEventGroups;

            if (theEvent == null)
            {
                theEvent = new CalendarEvent();
            }else
            {
                foreach (var refObject in theEvent.EventGroupEvents)
                {
                    refObject.Event = null;
                }
            }
            

            return View(theEvent);
        }
    }
}
