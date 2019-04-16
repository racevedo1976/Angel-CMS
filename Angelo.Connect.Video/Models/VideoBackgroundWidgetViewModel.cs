using Angelo.Connect.Widgets;
using Angelo.Connect.Video.Models.Validation;

namespace Angelo.Connect.Video.Models
{
    public class VideoBackgroundWidgetViewModel : IWidgetModel
    {
        public string Id { get; set; }
        public string VideoSourceType { get; set; }
        public string YoutubeVideoId { get; set; }
        public string VimeoVideoId { get; set; }
        [VideoBackgroundSource]
        public string SourceUri {get;set;}
        public string Positioning { get; set; }
        public bool Autoplay { get; set; }
        public bool ShowPlayerControls { get; set; }
    }
}
