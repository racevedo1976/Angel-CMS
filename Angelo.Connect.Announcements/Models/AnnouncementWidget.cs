using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Announcement.Models
{
    public class AnnouncementWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int PageSize { get; set; }
        public bool CreateAnnouncement { get; set; }
        public string AnnouncementId { get; set; }

        public List<AnnouncementWidgetCategory> Categories { get; set; }
        //public List<AnnouncementWidgetConnectionGroup> ConnectionGroups { get; set; }
        public List<AnnouncementWidgetTag> Tags { get; set; }
    }
}
