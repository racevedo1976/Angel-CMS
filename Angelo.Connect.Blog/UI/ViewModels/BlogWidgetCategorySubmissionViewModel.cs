using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Angelo.Connect.Models;
using Angelo.Connect.Blog.Models;

namespace Angelo.Connect.Blog.UI.ViewModels
{
    public class BlogWidgetCategorySubmissionViewModel
    {
        public string WidgetId { get; set; }
        public string Categories { get; set; }
    }
}
