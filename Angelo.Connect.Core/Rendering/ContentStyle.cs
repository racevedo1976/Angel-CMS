using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class ContentStyle
    {
        public bool FullWidth { get; set; } = false;

        public string BackgroundClass { get; set; }

        public string NodeClasses { get; set; }

        public string PaddingTop { get; set; }

        public string PaddingBottom { get; set; }

        public string MaxHeight { get; set; }

        public string Alignment { get; set; }
        
        public ContentStyle()
        {

        }

        public static ContentStyle DefaultStyle
        {
            get {
                return new ContentStyle();
            }
        }

        public static ContentStyle DefaultRootStyle
        {
            get {
                return new ContentStyle
                {
                    FullWidth = false,
                    PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                    PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
                };
            }
        }

    }
}
