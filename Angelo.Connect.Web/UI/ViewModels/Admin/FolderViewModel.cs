using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class FolderViewModel
    {
        [Display(Name = "Folder Id")]
        public string Id { get; set; }

        //[Display(Name = "UserName.Name")]
        //[StringLength(maximumLength: 50, ErrorMessage = "Name.Error.MaxLength")]
        //[Required(ErrorMessage = "Name.Error.Required")]
        public string OwnerUserId { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public string ParentFolderId { get; set; }
        public string ParentFolder { get; set; }

        public ICollection<FolderViewModel> ChildFolders { get; set; }
        public ICollection<DocumentViewModel> Documents { get; set; }
    }
}
