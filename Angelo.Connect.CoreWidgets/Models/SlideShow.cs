using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class SlideShow : IWidgetModel
    {
        public string Id { get; set; }
        public string Image1Src { get; set; }
        public string Image1Caption { get; set; }
        public string Image2Src { get; set; }
        public string Image2Caption { get; set; }
        public string Image3Src { get; set; }
        public string Image3Caption { get; set; }
        public int Speed { get; set; }
    }
}
