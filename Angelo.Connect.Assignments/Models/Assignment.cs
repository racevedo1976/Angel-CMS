using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Assignments.Models
{
    public class Assignment
    {
        public string Id { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedUTC { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public string AssignmentBody { get; set; }
        public DateTime DueUTC { get; set; }
        public string TimeZoneId { get; set; }
        public string Status { get; set; }
        public bool AllowComments { get; set; }
        public bool SendNotification { get; set; }
        public string NotificationId { get; set; }

        public List<AssignmentCategory> Categories;
        public List<UserGroup> UserGroups;

        public Assignment()
        {
            Categories = new List<AssignmentCategory>();
            UserGroups = new List<UserGroup>();
        }
    }

}

