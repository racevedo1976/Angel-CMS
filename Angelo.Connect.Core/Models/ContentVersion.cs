using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class ContentVersion : IContentVersion, IContentDescriptor
    {
        public string ContentType { get; set; }

        public string ContentId { get; set; }

        public string VersionCode { get; set; }

        public string VersionLabel { get; set; }

        public string UserId { get; set; }

        public DateTime Created { get; set; }

        public ContentStatus Status { get; set; }

        public string JsonData { get; set; }

        public ContentVersion()
        {
            Created = DateTime.Now;
            VersionCode = CreateVersionCode(DateTime.Now);
        }

        public ContentVersion(string contentType, string contentId, ContentStatus status = ContentStatus.Draft) : this()
        {
            ContentType = contentType;
            ContentId = contentId;
            Status = status;
        }


        public static string CreateVersionCode(DateTime timestamp)
        {
            return timestamp.ToString("yyyyMMdd-HHmmssFF");
        }
    }
}
