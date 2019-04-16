using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.SlideShow.Models
{
    public class SlideLayer
    {
        public string SlideId { get; set; }
        public Slide Slide { get; set; }

        public string LayerType { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string SourceUrl { get; set; }
        public string Target { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int? Height { get; set;}
        public int? Width { get; set; }
        public string BgColor { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
        public string TextDecoration { get; set; }

        public string Transition { get; set; }
        public string Position { get; set; }
        public string Delay { get; set; }

        #region Settings
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public string Color { get; set; }
        public Alignment HorizontalAlignment { get; set; }
        public Alignment VerticalAlignment { get; set; }

        #region Fades
        public Transition FadeInTransition { get; set; }
        public Direction FadeInDirection { get; set; }
        public int FadeInDuration { get; set; }
        public int FadeInDelay { get; set; }
        public Transition FadeOutTransition { get; set; }
        public Direction FadeOutDirection { get; set; }
        public int FadeOutDuration { get; set; }
        public int FadeOutDelay { get; set; }
        #endregion // Fades
        #endregion // Settings
    }
}
