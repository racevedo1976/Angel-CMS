using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Abstractions
{
    public interface IContentVersion
    {
        string VersionCode { get; set; }
        string VersionLabel { get; set; }
        string UserId { get; set; }
        DateTime Created { get; set; }
        ContentStatus Status { get; set; }
    }
}
