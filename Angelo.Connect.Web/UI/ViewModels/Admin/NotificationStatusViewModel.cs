using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NotificationStatusViewModel
    {
        public string Id { get; set; }
        public string LastStatus { get; set; }
        public string CurrentStatus { get; set; }
        public string CurrentStatusName { get; set; }
    }
}
