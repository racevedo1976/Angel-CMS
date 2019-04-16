using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarEventTags : ViewComponent
    {
        private CalendarQueryService _calendarQueryService;

        public CalendarEventTags(CalendarQueryService calendarQueryService)
        {
            _calendarQueryService = calendarQueryService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
           
            //Get Event or set default
            ViewData["eventId"] = id;

            var taglist = _calendarQueryService.GetEventTags(id);

            return View(taglist);
        }
    }
}
