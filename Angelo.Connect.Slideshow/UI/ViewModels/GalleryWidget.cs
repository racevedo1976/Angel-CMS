using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.SlideShow.Models
{
    public class GalleryWidgetViewModel : IWidgetModel
    {
        public string Id { get; set; }
        public string SiteId { get; set; }
        public string Title { get; set; }
        public string GalleryId { get; set; }
        public List<GalleryItem> GalleryItems { get; set; }

        public GalleryItem NewGalleryItem { get; set; }
    }
}
