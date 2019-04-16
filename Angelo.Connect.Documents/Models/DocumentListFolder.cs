using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Documents.Models
{
    public class DocumentListFolder
    {
        public DocumentListFolder()
        {
            Documents = new List<DocumentListDocument>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string WidgetId { get; set; }
        public int Sort { get; set; }
        public IList<DocumentListDocument> Documents { get; set; }
        public DocumentListWidget Widget { get; set; }
    }
}
