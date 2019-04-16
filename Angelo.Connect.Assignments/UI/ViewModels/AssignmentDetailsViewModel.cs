using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System;
using System.Collections.Generic;

namespace Angelo.Connect.Assignments.UI.ViewModels
{
    public class AssignmentDetailsViewModel
    {
        public AssignmentDetailsViewModel()
        {
            CategoryIds = new List<string>();
            Categories = new List<AssignmentCategory>();
            ConnectionGroupIds = new List<string>();
            ConnectionGroups = new List<UserGroup>();
        }

        public string Id { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedUTC { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public string AssignmentBody { get; set; }
        public string DueDate { get; set; }
        public string DueTime { get; set; }
        public string TimeZoneId { get; set; }
        public string TimeZoneName { get; set; }
        public string Status { get; set; }
        public bool AllowComments { get; set; }
        public bool SendNotification { get; set; }
        public string NotificationId { get; set; }

        public List<string> CategoryIds { get; set; }
        public List<AssignmentCategory> Categories { get; set; }
        public List<string> ConnectionGroupIds { get; set; }
        public List<UserGroup> ConnectionGroups { get; set; }

        public DateTime DueDT
        {
            get { return GetDueDT(); }
            set { SetDueDT(value); }
        }

        private DateTime GetDueDT()
        {
            var result = new DateTime(1900, 1, 1);
            DateTime date;
            DateTime time;
            if (DateTime.TryParse(DueDate, out date))
                if (DateTime.TryParse(DueTime, out time))
                   result = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
            return result;
        }

        private void SetDueDT(DateTime dt)
        {
            DueDate = dt.ToString("d");
            DueTime = dt.ToString("t");
        }

    }
}
