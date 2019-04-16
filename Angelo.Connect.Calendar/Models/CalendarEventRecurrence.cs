using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarEventRecurrence
    {
        public string Id { get; set; }
        public string Frequency { get; set; }  //Daily, Monthly,  Yearly
        public int Interval { get; set; }   // # applied to frequency. Every 1 day or Every 1 week.
        public DateTime? EndDate { get; set; } // Until when this recurrence is valid. When it stops.
        public int? Count { get; set; } // optional. How many times before it ends.
        public string DaysOfWeek { get; set; }  // optional. Days of the week the event will occur. example: 0;3;  (Sun;Wed;)
        public int? DayOfMonth { get; set; }    // day number of the month
        public string EventId { get; set; }
        public string Months { get; set; }
        public CalendarEvent Event { get; set; }
    }
}
