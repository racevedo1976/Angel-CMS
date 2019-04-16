using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Models
{
    public class ContentNode
    {
        public string Id { get; set; }
        public string ContentTreeId { get; set; }
        public string ParentId { get; set; }
        public string Zone { get; set; }
        public int Index { get; set; }
        public string WidgetType { get; set; }
        public string WidgetId { get; set; }
        public string ViewId { get; set; }

        public string JsonStyle { get; set; }

        public bool Locked { get; set; }

        public ContentTree ContentTree { get; set; }
        public ICollection<ContentNode> ChildNodes { get; set; }
        public ContentNode ParentNode { get; set; }

        // TODO: Remove
        // Depricated - not allowing widgets to be resized. Rather, will be full width inside it's zone. 
        // Special zone widgets now control sizing
        public string CssColumnSize { get; set; } /* not used anymore - everything is full width relative to parent container */


        // Constructors
        public ContentNode()
        {

        }

        public ContentNode(ContentStyle style) : this()
        {
            SetStyle(style);
        }

        public ContentStyle GetStyle()
        {
            return !string.IsNullOrEmpty(JsonStyle)
                   ? JsonConvert.DeserializeObject<ContentStyle>(JsonStyle)
                   : new ContentStyle();
        }

        public void SetStyle(ContentStyle style)
        {
            if (style != null)
                JsonStyle = JsonConvert.SerializeObject(style);
            else
                JsonStyle = null;
        }

    }
}
