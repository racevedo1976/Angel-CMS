using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NotificationLogEntryViewModel
    {
        public int Id { get; set; }
        public DateTime SentDT { get; set; }
        public string ToAddress { get; set; }
        public string LogEntryType { get; set; }
    }
}
