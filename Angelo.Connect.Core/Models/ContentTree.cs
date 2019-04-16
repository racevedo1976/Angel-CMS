using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class ContentTree : IContentDescriptor
    {
        public string Id { get; set; }

        public string ContentId { get; set; }

        public string ContentType { get; set; }

        public string VersionCode { get; set; }

        public List<ContentNode> ContentNodes { get; set; }

        // empty constructor
        public ContentTree()
        {
            Id = Guid.NewGuid().ToString("N");
            ContentNodes = new List<ContentNode>();
        }

        // Init from version meta
        public ContentTree(ContentVersion version) : this()
        {
            ContentType = version.ContentType;
            ContentId = version.ContentId;
            VersionCode = version.VersionCode;
        }

    }
}
