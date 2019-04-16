using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Models
{
    public class BlogWidgetTag
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string TagId { get; set; }

        public BlogWidget Widget { get; set; }
    }
}
