using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarWidgetSetting : IWidgetModel
    {
        public string Id { get; set; }
        public string CalendarId { get; set; }
        public string Title { get; set; }
        public string DefaultView { get; set; }
        public string StartDayOfWeek { get; set; }
        public bool Format12 { get; set; }
        public string SiteId { get; set; }
        public bool HideWeekends { get; set; }

        public bool MonthView { get; set; }
        public bool WeekView { get; set; }
        public bool DayView { get; set; }
        public bool ListView { get; set; }
       
    }
}
