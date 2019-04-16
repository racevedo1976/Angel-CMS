using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Models
{
    public class BlogCategory
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string Title { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        public IEnumerable<BlogWidgetCategory> BlogWidgetMap { get; set; }
        public IEnumerable<BlogPostCategory> BlogPostMap { get; set; }
    }
}
