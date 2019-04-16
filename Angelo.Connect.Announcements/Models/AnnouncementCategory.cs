using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.Models
{
    public class AnnouncementCategory
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string Title { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        public IEnumerable<AnnouncementWidgetCategory> AnnouncementWidgetMap { get; set; }
        public IEnumerable<AnnouncementPostCategory> AnnouncementPostMap { get; set; }
    }
}
