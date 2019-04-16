using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Angelo.Connect.Models;
using Angelo.Connect.Announcement.Models;

namespace Angelo.Connect.Announcement.UI.ViewModels
{
    public class AnnouncementWidgetCategorySubmissionViewModel
    {
        public string WidgetId { get; set; }
        public string Categories { get; set; }
    }
}
