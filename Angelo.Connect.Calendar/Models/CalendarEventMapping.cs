using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Views.ViewModel;
using Angelo.Identity;

namespace Angelo.Connect.Calendar.Models
{
    public static class CalendarEventMapping
    {

        public static EventViewModel ToEventViewModel(this CalendarEvent calendarEvent, UserManager userManager)
        {
            return new EventViewModel
            {
                id = calendarEvent.EventId,
                title = calendarEvent.Title,
                //url = "http://www.example.com",
                allDay = calendarEvent.AllDayEvent,

                //for all day events, calendar doesnt like time on dates.
                start = (calendarEvent.AllDayEvent ? calendarEvent.EventStart.ToString("yyyy-MM-dd") : calendarEvent.EventStart.ToString("o")) ,
                end = (calendarEvent.AllDayEvent ? calendarEvent.EventEnd.ToString("yyyy-MM-dd") : calendarEvent.EventEnd.ToString("o")),

                className = calendarEvent.Style,
                phone = calendarEvent.Phone,
                startEditable = false,
                editable = false,
                resourceEditable = false,
                durationEditable = false,
                description = calendarEvent.Description, // WebUtility.HtmlDecode(calendarEvent.Description),
                backgroundColor = calendarEvent.BackgroundColor,
                isRecurrent = calendarEvent.IsRecurrent,
                location = calendarEvent.Location,
                userId = calendarEvent.UserId,
                userName = (userManager.GetUserAsync(calendarEvent.UserId)).Result.DisplayName,
                showOrganizerName = calendarEvent.ShowOrganizerName,
                showPhoneNumber = calendarEvent.ShowPhoneNumber
        };

            
        }

        public static IList<EventViewModel> ToEventViewModel(this IList<CalendarEvent> calendarEvents, UserManager userManager)
        {
            return calendarEvents.Select(x => x.ToEventViewModel(userManager)).ToList();

           
        }
    }
}
