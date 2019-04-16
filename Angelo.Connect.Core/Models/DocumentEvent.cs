using Angelo.Connect.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class DocumentEvent
    {
        public int Id { get; set; }
        public string DocumentId { get; set; }
        public string DbLogEventId { get; set; }

        // Taking this out due to logs being in another DbContext now (otherwise requires the same tabe defs duplicated in Connect and Log)
        //public FileDocument Document { get; set; }

        public DbLogEvent Event { get; set; }
    }
}
