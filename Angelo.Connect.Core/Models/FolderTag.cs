using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class FolderTag : IAuditTrail
    {
        public string Id { get; set; }
        public string FolderId { get; set; }
        public string TagName { get; set; }
        public string CreatedBy { get; set; }

        public Folder Folder { get; set; }
    }
}
