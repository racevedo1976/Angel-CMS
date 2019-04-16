using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class Notification
    {
        public string Id { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedUTC { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public bool SendEmail { get; set; }
        public bool SendSms { get; set; }
        public string EmailHeaderId { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        public string SmsMessage { get; set; }
        public DateTime ScheduledUTC { get; set; }
        public string TimeZoneId { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }
        public string ProcId { get; set; }
        public int RetryCount { get; set; }
        public DateTime ProcStartUTC { get; set; }
        public DateTime ProcStopUTC { get; set; }
    }

}
