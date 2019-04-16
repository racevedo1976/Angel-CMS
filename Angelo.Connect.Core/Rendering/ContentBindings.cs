using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Rendering
{
    public class ContentBindings
    {
        public const string GENERIC_CONTENT_TYPE = "Generic";

        public string ContentType { get; set; }

        public string ContentId { get; set; }

        public string VersionCode { get; set; }

        public bool Editable { get; set; }

        public string ViewPath { get; set; }

        public object ViewModel { get; set; }

        public ContentBindings()
        {
            /* empty constructor */
        }

        public ContentBindings(IContentDescriptor contentInfo)
        {
            ContentType = contentInfo.ContentType;
            ContentId = contentInfo.ContentId;
            VersionCode = contentInfo.VersionCode;
        }

        public ContentBindings(IContentDescriptor contentInfo, string contentViewPath, object contentViewModel, bool editable = false) : this(contentInfo)
        {
            ViewPath = contentViewPath;
            ViewModel = contentViewModel;
            Editable = editable;
        }

        public static ContentBindings Generic(string contentViewPath, object contentViewModel)
        {
            return new ContentBindings
            {
                ContentType = GENERIC_CONTENT_TYPE,
                ViewPath = contentViewPath,
                ViewModel = contentViewModel
            };
        }
    }
}
