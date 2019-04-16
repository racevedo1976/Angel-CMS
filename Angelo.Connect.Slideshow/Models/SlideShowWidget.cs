using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.SlideShow.Models
{
    // TODO: Maybe have a SlideShowFolderManager so handle the custom DB fields
    public class SlideShowWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string SiteId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public Transition Transition { get; set; }
        public string BackgroundColor { get; set; }
        public string Height { get; set; }
        //public string SlideShowId { get; set; }
        //public SlideShow SlideShow { get; set; }

        #region Defaults
        #region ProgressBar
        //public bool DefaultProgressBarIsActive { get; set; }

        //public Position DefaultProgressBarPosition { get; set; }

        //public int DefaultProgressBarHeight { get; set; }

        //public int DefaultProgressBarOpacity { get; set; }

        //public string DefaultProgressBarColor { get; set; }
        #endregion // ProgressBar
        #region SlideShow
        //public int DefaultSlideShowDuration { get; set; }
        //public int DefaultSlideShowInitializationDelay { get; set; }
        //public Transition DefaultSlideShowTransitions { get; set; }
        //public Size DefaultSlideShowImageSourceSize { get; set; }

        //public bool DefaultSlideShowIsPausedOnHover { get; set; }

        //public bool DefaultSlideShowIsLooped { get; set; }

        //public int DefaultSlideShowMaximumLoops { get; set; }

        //public bool DefaultSlideShowIsRandomMode { get; set; }

        //public ShadowType DefaultSlideShowShadowType { get; set; }

        //public Size DefaultSlideShowDottedOverlaySize { get; set; }
        //public string DefaultSlideShowImageUrl { get; set; }    // TODO: A collection instead (for parallax)?

        //public string DefaultSlideShowBackgroundColor { get; set; }
        //public int DefaultSlideShowPadding { get; set; }

        //public Position DefaultSlideBackgroundShowPosition { get; set; }
        //public string DefaultSlideShowTiling { get; set; }
        #endregion // SlideShow 
        #endregion // Defaults
    }
}
