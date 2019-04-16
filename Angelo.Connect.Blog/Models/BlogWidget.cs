using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.Blog.Models
{
    public class BlogWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int PageSize { get; set; }
        public bool CreateBlog { get; set; }
        public string BlogId { get; set; }

        public List<BlogWidgetCategory> Categories { get; set; }
        //public List<BlogWidgetConnectionGroup> ConnectionGroups { get; set; }
        public List<BlogWidgetTag> Tags { get; set; }
    }
}
