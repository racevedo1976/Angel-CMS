namespace Angelo.Connect.News.Models
{
    public class NewsWidgetCategory
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string CategoryId { get; set; }

        public NewsWidget Widget { get; set; }

        public NewsCategory Category { get; set; }
    }
}
