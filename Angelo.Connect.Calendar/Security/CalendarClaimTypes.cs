using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Security
{
    public class CalendarClaimTypes
    {
        // primary author permissions (eg, can create personal calendar events and groups)
        public const string CalendarAuthor = "calendar-author"; 

        // permissions that can be delegated by the author
        public const string CalendarEventGroupContribute = "calendar-event-group-contribute";
        public const string CalendarEventContribute = "calendar-event-contribute";
    }
}
