using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Abstractions
{
    public interface IFolderItem
    {
        string Id { get; set; }
        string FolderId { get; set; }
        string DocumentId { get; set; }

        ModerationStatus ItemStatus { get; set; }
    }
}
