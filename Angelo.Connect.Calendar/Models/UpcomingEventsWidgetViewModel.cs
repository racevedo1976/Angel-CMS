using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Calendar.Models
{
    public class UpcomingEventsWidgetViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int PostsToDisplay { get; set; }
        public string TextColor { get; set; }
        public string DisplayTime { get; set; }
        public string DisplayDate { get; set; }
        public string Description { get; set; }
    }
}
