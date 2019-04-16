using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarEventGroup
    {
        public CalendarEventGroup() { }
        [ScaffoldColumn(false)]
        public string EventGroupId { get; set; }
        [ScaffoldColumn(false)]
        public string UserId { get; set; }
        [ScaffoldColumn(false)]
        public string SiteId { get; set; }
        public string Title { get; set; }

        public ICollection<CalendarEventGroupEvent> EventGroupEvents { get; set; }
        public ICollection<CalendarWidgetEventGroup> WidgetGroups { get; set; }

    }
}
