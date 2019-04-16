using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Announcement.UI.ViewModels
{
    public class AnnouncementWidgetTagFormViewModel
    {
        public AnnouncementWidgetTagFormViewModel()
        {
            Tags = new List<Tag>();
        }
        public string WidgetId { get; set; }
        public bool AllTags { get; set; }

        public IEnumerable<Tag> Tags { get; set; }
        public IEnumerable<string> SelectedTagIds { get; set; }
    }
}
