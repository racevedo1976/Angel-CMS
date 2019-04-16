using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Rendering
{
    public class VersionInfo : IContentVersion
    {
        public DateTime Created { get; set; }

        public ContentStatus Status { get; set; }

        public string UserId { get; set; }

        public string VersionCode { get; set; }

        public string VersionLabel { get; set; }
    }
}
