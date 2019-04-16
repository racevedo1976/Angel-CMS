using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NotificationUnsubscribeCode
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string NotificationId { get; set; }
        public string Code { get; set; }
        public DateTime ExpirationUTC { get; set; }
        public bool Confirmed { get; set; }
        public int FailureCount { get; set; }
        public string Email { get; set; }
        public DateTime SentEmailUTC { get; set; }
        public string SmsNumber { get; set; }
        public DateTime SentSmsUTC { get; set; }

    }
}
