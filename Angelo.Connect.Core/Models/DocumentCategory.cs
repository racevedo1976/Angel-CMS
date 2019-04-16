using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class DocumentCategory : IAuditTrail
    {
        public string DocumentTagId { get; set; }
        public string DocumentId { get; set; }
        public string CategoryId { get; set; }
        public string CreatedBy { get; set; }

        public Category Category { get; set; }
    }
}
