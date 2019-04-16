using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class FileExplorerViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public string FileType { get; set; }
        public string ObjectType { get; set; }
        public string ParentFolderId { get; set; }
        public string FileUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string Extension { get; set; }
        public string CreatedDateString { get; set; }
    }
}
