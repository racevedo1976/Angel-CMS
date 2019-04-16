using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Announcement.Models;

namespace Angelo.Connect.Announcement.UI.ViewModels
{
    public class AnnouncementWidgetCategoryFormViewModel
    {
        public AnnouncementWidgetCategoryFormViewModel()
        {
            AnnouncementCategories = new List<AnnouncementCategory>();
        }
        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public IEnumerable<AnnouncementCategory> AnnouncementCategories { get; set; }
        public IEnumerable<string> SelectedCategoryIds { get; set; }
    }
}
