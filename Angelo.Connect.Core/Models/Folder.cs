using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Models
{
    public class Folder : IFolder, IAuditTrail, IContentType
    {
        public Folder()
        {
            ChildFolders = new List<Folder>();
            IsSystemFolder = false;
            IsDeleted = false;
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string ParentId { get; set; }

        public FolderFlag FolderFlags { get; set; }
        public OwnerLevel OwnerLevel { get; set; }

        public string DocumentType { get; set; }
        public string FolderType { get; set; }
        
        public string OwnerId { get; set; }
        public string CreatedBy { get; set; }
        public bool IsSystemFolder { get; set; }
        public bool IsDeleted { get; set; }
        public string DocumentLibraryId { get; set; }
        public DocumentLibrary DocumentLibrary { get; set; }

        public Folder ParentFolder { get; set; }
        public ICollection<Folder> ChildFolders { get; set; }
        public ICollection<ResourceClaim> Security { get; set; }
        public ICollection<FolderSharing> Sharing { get; set; }
        public ICollection<FolderCategory> CategoryMap { get; set; }
        public ICollection<FolderTag> Tags { get; set; }
        public ICollection<FolderItem> Items { get; set; }
    }
}
