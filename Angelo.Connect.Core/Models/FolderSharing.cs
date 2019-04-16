using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class FolderSharing : IAuditTrail
    {
        public string FolderId { get; set; }
        public string SharedFolderId { get; set; }

        public string CreatedBy { get; set; }

        public Folder Folder { get; set; }
        public Folder SharedFolder { get; set; }


    }
}
