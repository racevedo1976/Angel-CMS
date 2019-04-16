using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarEventManager : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;
        public CalendarEventManager(CalendarQueryService calendarQueryService)
        {
            _calendarQueryService = calendarQueryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string eventId, string day, string hour)
        {
            var calEvent = new CalendarEvent();

            if (string.IsNullOrEmpty(eventId))
            {
                calEvent = _calendarQueryService.GetDefaultModel();
                if (!string.IsNullOrEmpty(day))
                {
                    //fix selected date and time before parsing
                    calEvent.EventStart = DateTime.Parse(day.Replace('~', '/') + ' ' + hour.Replace('~', ' '));
                    calEvent.EventEnd = calEvent.EventStart.AddMinutes(30);
                }
                
            }
            else
            {
                calEvent = _calendarQueryService.GetEventById(eventId);
            }

            foreach (var refObject in calEvent.EventGroupEvents)
            {
                refObject.Event = null;

            }
            return View(calEvent);
        }
    }
}
