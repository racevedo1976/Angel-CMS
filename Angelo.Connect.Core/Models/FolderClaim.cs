using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Models
{
    public class FolderClaim : IContentClaim, IAuditTrail
    {
        public string Id { get; set; }    // EF required
        public string FolderId { get; set; }
        public string ClaimType { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        
        public string CreatedBy { get; set; }

        public Folder Folder { get; set; }
    }
}
