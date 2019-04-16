using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.UI.ViewModels
{
    public class UpcomingEventsGroupFormViewModel
    {
        public UpcomingEventsGroupFormViewModel()
        {
            UpcomingEventGroups = new List<CalendarEventGroup>();
        }
        public string WidgetId { get; set; }
        public string UserId { get; set; }

        public IEnumerable<CalendarEventGroup> UpcomingEventGroups { get; set; }
        public IEnumerable<string> SelectedGroupIds { get; set; }
    }
}
