using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Blog.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.Blog.UI.ViewModels
{
    public class UserBlogPostsViewModel
    {
        public UserBlogPostsViewModel()
        {
            UserBlogPosts = new List<BlogPost>();
        }

        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public List<BlogPost> UserBlogPosts { get; set; }
    }
}
