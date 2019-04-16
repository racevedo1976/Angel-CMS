using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NotificationListItemViewModel
    {
        public string Id { get; set; }
        public DateTime CreatedDT { get; set; }
        public DateTime ScheduledDT { get; set; }
        public string TimeZoneId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
    }
}
