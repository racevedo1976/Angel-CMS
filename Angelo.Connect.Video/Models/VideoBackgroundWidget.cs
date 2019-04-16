using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;
using Angelo.Connect.Models;

namespace Angelo.Connect.Video.Models
{
    public class VideoBackgroundWidget
    {
        public string Id { get; set; }
        public string VideoSourceType { get; set; }
        public string YoutubeVideoId { get; set; }
        public string VimeoVideoId { get; set; }
        public string VideoUrl { get; set; }
        public string Positioning { get; set; }
        public bool Autoplay { get; set; }
        public bool ShowPlayerControls { get; set; }
    }
}
