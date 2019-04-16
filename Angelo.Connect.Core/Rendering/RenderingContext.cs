using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Rendering
{
    public class RenderingContext
    {
        public string ContentType { get; set; }
        public string ContentId { get; set; }
        public string ContentVersionCode { get; set; }
        public bool ContentEditable { get; set; }
        public string ContentViewPath { get; set; }
        public object ContentViewModel { get; set; }
        public VersionInfo ContentVersionInfo { get; set; }
        public string ContentTreeId { get; set; }

        public ToolbarSettings Toolbar { get; set; }

        public RenderingContext()
        {
            /* Empty Constructor */
        }

        public RenderingContext(ContentBindings contentBindings)
        {
            ContentType = contentBindings.ContentType;
            ContentId = contentBindings.ContentId;
            ContentVersionCode = contentBindings.VersionCode;
            ContentEditable = contentBindings.Editable;
            ContentViewPath = contentBindings.ViewPath;
            ContentViewModel = contentBindings.ViewModel;
        }

        public virtual bool LoadDesignTools
        {
            get {
                return ContentEditable;
            }
        }

    }
}
