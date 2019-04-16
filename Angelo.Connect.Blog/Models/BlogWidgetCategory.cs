using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Models
{
    public class BlogWidgetCategory
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string CategoryId { get; set; }

        public BlogWidget Widget { get; set; }

        public BlogCategory Category { get; set; }
    }
}
