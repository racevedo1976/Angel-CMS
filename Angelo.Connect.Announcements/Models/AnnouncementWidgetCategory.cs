using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.Models
{
    public class AnnouncementWidgetCategory
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string CategoryId { get; set; }

        public AnnouncementWidget Widget { get; set; }

        public AnnouncementCategory Category { get; set; }
    }
}
