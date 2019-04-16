using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Carousel.Models
{
    public class CarouselSlide
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public string LinkTarget { get; set; }
        public int Sort { get; set; }
        public CarouselWidget Widget { get; set; }
    }
}
