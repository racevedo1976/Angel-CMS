using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.Models
{
    public class AnnouncementWidgetTag
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string TagId { get; set; }

        public AnnouncementWidget Widget { get; set; }
    }
}
