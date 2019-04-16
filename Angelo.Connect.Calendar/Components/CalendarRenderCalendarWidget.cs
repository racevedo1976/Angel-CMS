using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Security;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarRenderCalendarWidget : ViewComponent
    {
        private CalendarSecurityService _calendarSecurity;

        public CalendarRenderCalendarWidget(CalendarSecurityService calendarSecurity)
        {
            _calendarSecurity = calendarSecurity;
        }

        public async Task<IViewComponentResult> InvokeAsync(CalendarWidgetSetting id)
        {
            _calendarSecurity.AuthorizeForCreateEvents();

            return View();
        }
    }
}
