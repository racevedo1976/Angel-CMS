using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Models
{
    public class ContentClaim : IContentClaim
    {
        public string ContentType { get; set; }
        public string ContentId { get; set; }
        public string ClaimType { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
    }
}
