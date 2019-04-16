using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Rendering
{
    public class ShellContext : RenderingContext
    {
        public string WindowTitle { get; set; }
        
        public Dictionary<string, string> MetaTags { get; set; }

        public PageTemplate MasterPageTemplate { get; set; }

        public string MasterPageId { get; set; }
    
        public string MasterPageTreeId { get; set; }

        public bool MasterPageEditable { get; set; }

        public VersionInfo MasterPageVersionInfo { get; set; }

        public override bool LoadDesignTools
        {
            get {
                return ContentEditable || MasterPageEditable;
            }
        }

        public ShellContext(RenderingContext baseContext)
        {
            ContentType = baseContext.ContentType;
            ContentId = baseContext.ContentId;
            ContentVersionCode = baseContext.ContentVersionCode;
            ContentEditable = baseContext.ContentEditable;
            ContentViewPath = baseContext.ContentViewPath;
            ContentViewModel = baseContext.ContentViewModel;
            ContentVersionInfo = baseContext.ContentVersionInfo;
            ContentTreeId = baseContext.ContentTreeId;

            Toolbar = baseContext.Toolbar;
        }

    }
}
