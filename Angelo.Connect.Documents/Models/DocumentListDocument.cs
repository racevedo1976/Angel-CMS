using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Documents.Models
{
    public class DocumentListDocument
    {
        public string Id { get; set; }
        public string WidgetId { get; set; }
        public string DocumentId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FolderId { get; set; }
        public int Sort { get; set; }
        public DocumentListFolder Folder { get; set; }
        public DocumentListWidget Widget { get; set; }
    }
}
