using System;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class DocumentViewModel
    {
        [Display(Name = "Document Id")]
        public string DocumentId { get; set; }

        //[Display(Name = "UserName.Name")]
        //[StringLength(maximumLength: 50, ErrorMessage = "Name.Error.MaxLength")]
        //[Required(ErrorMessage = "Name.Error.Required")]
        public string FileName { get; set; }

        public string FileType { get; set; }

        public long ContentLength { get; set; }

        //public string FolderId { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public string FileExtension { get; set; }

        public string FileSize { get; set; }

        public string CreatedDate { get; set; }
        public string FolderId { get; set; }
        public string FolderOwnerId { get; set; }
    }
}
