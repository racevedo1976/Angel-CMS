using Microsoft.AspNetCore.Mvc;
using Angelo.Connect.Calendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarEventDisplayMode : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;
        public CalendarEventDisplayMode(CalendarQueryService calendarQueryService)
        {
            _calendarQueryService = calendarQueryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string eventId)
        {
            IList<string> testList = new List<string>();

            var calEvent =_calendarQueryService.GetEventById(eventId);

            return View(calEvent);
        }
    }
}
