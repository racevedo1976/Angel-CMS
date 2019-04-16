using System.Collections.Generic;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.News.Models
{
    public class NewsWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int PageSize { get; set; }
        public bool CreateNews { get; set; }
        public string NewsId { get; set; }

        public List<NewsWidgetCategory> Categories { get; set; }
        //public List<NewsWidgetConnectionGroup> ConnectionGroups { get; set; }
        public List<NewsWidgetTag> Tags { get; set; }
    }
}
