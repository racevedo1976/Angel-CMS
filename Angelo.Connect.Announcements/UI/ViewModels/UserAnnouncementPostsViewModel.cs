using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.Announcement.UI.ViewModels
{
    public class UserAnnouncementPostsViewModel
    {
        public UserAnnouncementPostsViewModel()
        {
            UserAnnouncementPosts = new List<AnnouncementPost>();
        }

        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public List<AnnouncementPost> UserAnnouncementPosts { get; set; }
    }
}
