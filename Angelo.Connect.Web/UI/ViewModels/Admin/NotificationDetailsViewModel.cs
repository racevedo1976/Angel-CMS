using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NotificationDetailsViewModel
    {
        public class ScheduleActions
        {
            public const string Draft = "draft";
            public const string SendNow = "sendnow"; 
            public const string Schedule = "schedule";
        }


        public NotificationDetailsViewModel()
        {
            NotificationGroupIds = new List<string>();
            NotificationGroups = new List<UserGroup>();
            ConnectionGroupIds = new List<string>();
            ConnectionGroups = new List<UserGroup>();
        }

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
        public string ScheduleAction { get; set; }
        public string ScheduledDate { get; set; }
        public string ScheduledTime { get; set; }
        public string TimeZoneId { get; set; }
        public string TimeZoneName { get; set; }
        public string Status { get; set; }
        public string ErrorMsg { get; set; }

        public DateTime ScheduledDT {
            get { return GetScheduledDT(); }
            set { SetScheduledDT(value); }
        }

        public List<string> NotificationGroupIds { get; set; }
        public List<UserGroup> NotificationGroups { get; set; }
        public List<string> ConnectionGroupIds { get; set; }
        public List<UserGroup> ConnectionGroups { get; set; }

        private DateTime GetScheduledDT()
        {
            var result = new DateTime(1900, 1, 1);
            DateTime date;
            DateTime time;
            if (DateTime.TryParse(ScheduledDate, out date))
                if (DateTime.TryParse(ScheduledTime, out time))
                   result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            return result;
        }

        private void SetScheduledDT(DateTime dt)
        {
            ScheduledDate = dt.ToString("d");
            ScheduledTime = dt.ToString("t");
        }

    }
}
