using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Logging
{
    public class LogSummary
    {
        // Not persisting this any more (would be a view, anyway)
        //public string Id { get; set; }

        public DateTime? Created { get; set; }

        public DateTime? LastRead { get; set; }
        public DateTime? LastDownload { get; set; }
        public DateTime? LastWrite { get; set; }
        
        public int ReadCount { get; set; }
        public int DownloadCount { get; set; }
        public int WriteCount { get; set; }

        // Only for logical (virtual) deletions
        public DateTime? Deleted { get; set; }

        public DocumentEvent DocumentLog { get; set; }
        public string DocumentLogId { get; set; }


        // Other user accesa
        public int ReadCountOther { get; set; }
        public DateTime? LastReadOther { get; set; }
        public string LastReadUserId { get; set; }

        public int WriteCountOther { get; set; }
        public DateTime? LastWriteOther { get; set; }
        public string LastWriteUserId { get; set; }

        public int DownloadCountOther { get; set; }
        public DateTime? LastDownloadOther { get; set; }
        public string LastDownloadUserId { get; set; }
    }
}
