using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NotificationSmsLog
    {
        public int Id { get; set; }
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string MobileNumber { get; set; }
        public string WirelessProviderId { get; set; }
        public DateTime SentUTC { get; set; }

        public Notification Notification;
    }
}
