using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarEditEventDetails : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;

        public CalendarEditEventDetails(CalendarQueryService calendarQueryService)
        {
            _calendarQueryService = calendarQueryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(CalendarEvent calEvent)
        {
            //var calEvent = new CalendarEvent();

            //Get Event or set default

            foreach (var refObject in calEvent.EventGroupEvents)
            {
                refObject.Event = null;

            }
            return View(calEvent);
        }
    }
}
