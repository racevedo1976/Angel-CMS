using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.Documents.Models
{
    public class DocumentListWidget : IWidgetModel
    {
        public string Id { get; set; }
        public string SiteId { get; set; }
        public string Title { get; set; }
        public List<DocumentListDocument> Documents { get; set; }
        public List<DocumentListFolder> Folders { get; set; }
    }
}
