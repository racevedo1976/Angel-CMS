using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;
using Angelo.Connect.Models;

namespace Angelo.Connect.Video.Models
{
    public class VideoStreamLink
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
    }
}
