using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Models
{
    public class ContentCategory
    {
        public string ContentType { get; set; }
        public string ContentId { get; set; }
        public string CategoryId { get; set; }
        
        public Category Category { get; set; }
    }
}
