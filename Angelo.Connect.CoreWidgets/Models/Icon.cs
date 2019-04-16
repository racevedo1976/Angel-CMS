using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class Icon : IWidgetModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Size { get; set; }

        public string Tooltip { get; set; }

        public string Text { get; set; }

        public string Url { get; set; }

        public bool UrlOpenNew { get; set; }
    }
}
