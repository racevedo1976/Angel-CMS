using Angelo.Connect.Calendar.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Interface
{
    public interface IRecurringEvent
    {
        CalendarEvent EventDetail { get; set; }
        DateTime From { get; set; }
        DateTime To { get; set; }
        string FrequencyType();
        IEnumerable<CalendarEvent> ResolveRecurringEvents();
        DateTime CalculateEndDate();
    }
}
