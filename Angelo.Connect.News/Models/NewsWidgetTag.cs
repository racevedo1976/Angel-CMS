namespace Angelo.Connect.News.Models
{
    public class NewsWidgetTag
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string TagId { get; set; }

        public NewsWidget Widget { get; set; }
    }
}
