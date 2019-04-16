using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class DocumentLibrary
    {
        public string Id { get; set; }
        public string LibraryType { get; set; }
        public string Location { get; set; }
        public string OwnerId { get; set; }

        public ICollection<Folder> Folders { get; set; }
    }
}
