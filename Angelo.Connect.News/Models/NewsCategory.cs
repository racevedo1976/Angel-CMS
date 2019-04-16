using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.News.Models
{
    public class NewsCategory
    {
        [ScaffoldColumn(false)]
        public string Id { get; set; }

        public string Title { get; set; }

        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [ScaffoldColumn(false)]
        public bool IsActive { get; set; }

        public IEnumerable<NewsWidgetCategory> NewsWidgetMap { get; set; }
        public IEnumerable<NewsPostCategory> NewsPostMap { get; set; }
    }
}
