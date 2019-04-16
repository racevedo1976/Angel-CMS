using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Rendering
{
    public class MasterPageSettings
    {
        public static string ContentType { get; } = typeof(PageMaster).FullName;

        public string MasterPageId { get; set; }
        public string VersionCode { get; set; }

        public bool Editable { get; set; }

        public MasterPageSettings() {  /* empty constructor */ }


        public MasterPageSettings(IContentDescriptor descriptor)
        {
            if (descriptor.ContentType != ContentType)
                throw new Exception($"Content Type {descriptor.ContentType} cannot be used to initialize {nameof(MasterPageSettings)}");

            MasterPageId = descriptor.ContentId;
            VersionCode = descriptor.VersionCode;
        }

        public MasterPageSettings(IContentDescriptor descriptor, bool editable) : this(descriptor)
        {
            Editable = editable;
        }
    }
}
