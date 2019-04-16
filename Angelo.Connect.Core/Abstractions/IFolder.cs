using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Abstractions
{
    public interface IFolder
    {
        string Id { get; set; }
        string Title { get; set; }
        string ParentId { get; set; }
        string DocumentType { get; set; }
        string FolderType { get; set; }
        bool IsSystemFolder { get; set; }
        bool IsDeleted { get; set; }
        FolderFlag FolderFlags { get; set; }

        OwnerLevel OwnerLevel { get; set; }
        string OwnerId { get; set; }
    }
}
