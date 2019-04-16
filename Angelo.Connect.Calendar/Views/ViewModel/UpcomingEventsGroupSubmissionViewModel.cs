using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Angelo.Connect.Models;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.UI.ViewModels
{
    public class UpcomingEventsGroupSubmissionViewModel
    {
        public string WidgetId { get; set; }
        public string Groups { get; set; }
    }
}
