using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Logging;

namespace Angelo.Connect.Logging
{
    public class LogDescriptor
    {
        public LogSummary Summary { get; set; }

        public IEnumerable<DbLogEvent> Events { get; set; }
    }
}
