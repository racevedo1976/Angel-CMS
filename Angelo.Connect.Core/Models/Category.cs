using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Security;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Models
{
    public class Category
    {
        public Category()
        {
            FolderMap = new List<FolderCategory>();
            DocumentMap = new List<DocumentCategory>();
            ContentMap = new List<ContentCategory>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public bool IsActive { get; set; }

        public ICollection<FolderCategory> FolderMap { get; set; }
        public ICollection<DocumentCategory> DocumentMap { get; set; }
        public ICollection<ContentCategory> ContentMap { get; set; }
    }
}
