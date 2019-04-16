using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.SlideShow.Models
{
    public class GalleryItem
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        //public string DocumentId { get; set; }
        //[Required(ErrorMessage = "Caption is required.")]
        public string Title { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        //public int Position { get; set; }

        public GalleryWidget Widget { get; set; }

        #region Link
        public bool IsLinkEnabled { get; set; }
        public string LinkUrl { get; set; }
        public LinkTarget LinkTarget { get; set; }
        #endregion // Link
    }
}
