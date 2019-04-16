using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Models
{
    public class BlogPostCategory
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        public string CategoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public BlogPost Post { get; set; }

        public BlogCategory Category { get; set; }
    }
}
