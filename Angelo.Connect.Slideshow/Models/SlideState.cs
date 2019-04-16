using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Models
{
    public enum SlideState
    {
        None = 0,   // Type is determined by the service at instantiation
        Published,
        Unpublished 
    }
}
