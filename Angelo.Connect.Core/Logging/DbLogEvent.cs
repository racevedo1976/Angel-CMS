using System;
using Microsoft.Extensions.Logging;

namespace Angelo.Connect.Logging
{
    public class DbLogEvent
    {
        public int Id { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Category { get; set; }
        public int EventCode { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
        public string ResourceId { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }

    }
}
