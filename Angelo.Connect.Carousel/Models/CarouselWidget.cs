using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.Carousel.Models
{
    public class CarouselWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string SiteId { get; set; }
        public string Title { get; set; }
        public List<CarouselSlide> Slides { get; set; }
        //public List<CarouselFolder> Folders { get; set; }
    }
}
