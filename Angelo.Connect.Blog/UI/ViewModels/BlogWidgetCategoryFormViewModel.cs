using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Blog.Models;

namespace Angelo.Connect.Blog.UI.ViewModels
{
    public class BlogWidgetCategoryFormViewModel
    {
        public BlogWidgetCategoryFormViewModel()
        {
            BlogCategories = new List<BlogCategory>();
        }
        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public IEnumerable<BlogCategory> BlogCategories { get; set; }
        public IEnumerable<string> SelectedCategoryIds { get; set; }
    }
}
