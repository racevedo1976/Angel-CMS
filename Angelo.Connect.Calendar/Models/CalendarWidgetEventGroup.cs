using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarWidgetEventGroup
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string Title { get; set; }
        public string EventGroupId { get; set; }
       // public CalendarEventGroup EventGroup { get; set; }

        public string Color { get; set; }
    }

}
