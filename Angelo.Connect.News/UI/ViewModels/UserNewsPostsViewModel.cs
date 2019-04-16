using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.News.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.News.UI.ViewModels
{
    public class UserNewsPostsViewModel
    {
        public UserNewsPostsViewModel()
        {
            UserNewsPosts = new List<NewsPost>();
        }

        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public List<NewsPost> UserNewsPosts { get; set; }
    }
}
