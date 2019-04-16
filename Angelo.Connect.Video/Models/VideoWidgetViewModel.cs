using Angelo.Connect.Widgets;

namespace Angelo.Connect.Video.Models
{
    public class VideoWidgetViewModel : IWidgetModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string VideoSourceType { get; set; }
        public string StreamId { get; set; }
        public string DocumentId { get; set; }
        public string YouTubeVideoId { get; set; }
        public string SourceName { get; set; }
        public string SourceUri {get;set;}
        public string VideoFileExt { get; set; }

        public string VideoId
        {
            get
            {
                switch (VideoSourceType)
                {
                    case VideoSourceTypes.Document: return DocumentId;
                    case VideoSourceTypes.Stream: return StreamId;
                    case VideoSourceTypes.YouTube: return YouTubeVideoId;
                    default: return string.Empty;
                }
            }
        }
    }
}
