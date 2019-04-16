using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using System.ComponentModel.DataAnnotations;


namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SharedFolderTreeNodeViewModel
    {
        public string NodeId { get; set; }
        public string ParentNodeId { get; set; }
        public SharedFolderTreeNodeType NodeType { get; set; }
        public string DocumentType { get; set; }
        public string FolderId { get; set; }

        [Display(Name = "Title.Name", ShortName = "Title.ShortName")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string Title { get; set; }
    }
}
