using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NotificationStatus
    {
        public const string Draft = "draft";
        public const string Scheduled = "scheduled";
        public const string Processing = "processing";
        public const string Sent = "sent";
        public const string Error = "error";
        public const string Canceled = "canceled";
    }
}
