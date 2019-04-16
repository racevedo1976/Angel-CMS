using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Calendar.Models
{
    public class UpcomingEventsWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int PostsToDisplay { get; set; }
        public string TextColor { get; set; }
        public bool UseTextColor { get; set; }

        //public ICollection<UpcomingEventsWidgetEventGroup> WidgetGroups { get; set; }

    }
}
