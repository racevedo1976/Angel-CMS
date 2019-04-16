using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SharedFolderViewModel
    {
        public string FolderId { get; set; }
        public string ParentFolderId { get; set; }

        [Display(Name = "Folder Title", ShortName = "Title")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string Title { get; set; }

        public FolderFlag FolderFlags { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string FolderType { get; set; }
        public string DocumentType { get; set; }
        public string OwnerId { get; set; }

        public string CreatedBy { get; set; }
    }
}

