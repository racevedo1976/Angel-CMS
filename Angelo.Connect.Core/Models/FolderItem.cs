using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class FolderItem : IFolderItem, IAuditTrail
    {
        public string Id { get; set; }
        public string FolderId { get; set; }
        public string DocumentId { get; set; }
        
        public ModerationStatus ItemStatus { get; set; }

        public bool InheritSecurity { get; set; }
        public bool InheritSharing { get; set; }
        public bool InheritTags { get; set; }
        public bool AllowComments { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }

        public Folder Folder { get; set; }
    }
}
