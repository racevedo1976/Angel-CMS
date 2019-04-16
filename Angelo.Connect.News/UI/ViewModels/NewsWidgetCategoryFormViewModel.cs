using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.News.Models;

namespace Angelo.Connect.News.UI.ViewModels
{
    public class NewsWidgetCategoryFormViewModel
    {
        public NewsWidgetCategoryFormViewModel()
        {
            NewsCategories = new List<NewsCategory>();
        }
        public string WidgetId { get; set; }

        public string UserId { get; set; }

        public IEnumerable<NewsCategory> NewsCategories { get; set; }
        public IEnumerable<string> SelectedCategoryIds { get; set; }
    }
}
