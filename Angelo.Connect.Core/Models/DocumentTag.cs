using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class DocumentTag : IAuditTrail
    {
        public string DocumentTagId { get; set; }
        public string DocumentId { get; set; }
        public string TagName { get; set; }
        public string CreatedBy { get; set; }
        
        public FileDocument Document { get; set; }
        public Tag Tag { get; set; }
    }
    //public class DocumentTag
    //{
    //    public DocumentTag()
    //    {
    //    }

    //    public string DocumentId { get; set; }
    //    public int TagId { get; set; }

    //    public IDocument Document { get; set; }

    //    public ICollection<IDocument> Documents { get; set; }
    //}
}
