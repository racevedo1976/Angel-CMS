using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class CategoryViewModel
    {
        public CategoryViewModel()
        {
            DocumentMap = new List<DocumentCategory>();
            FolderMap = new List<FolderCategory>();
        }

        [Display(Name = "Name", ShortName = "Name")]
        public string Id { get; set; }

        [Display(Name = "Title", ShortName = "Title")]
        public string Title { get; set; }

        [Display(Name = "Owner Level", ShortName = "Owner Level")]
        public OwnerLevel OwnerLevel { get; set; }

        [Display(Name = "Owner Id", ShortName = "Owner Id")]
        public string OwnerId { get; set; }

        [Display(Name = "Folder Map", ShortName = "Folder Map")]
        public ICollection<FolderCategory> FolderMap { get; set; }

        [Display(Name = "Document Map", ShortName = "Document Map")]
        public ICollection<DocumentCategory> DocumentMap { get; set; }
    }
}
