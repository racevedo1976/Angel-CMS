using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Models
{
    public enum SlideShowType
    {
        None = 0,   // Type is determined by the service at slideshow instantiation
        Standard,   // Ellipses menu is at bottom center of show
        Hero        // Main slide is the only content, no ellipses 
    }
}
