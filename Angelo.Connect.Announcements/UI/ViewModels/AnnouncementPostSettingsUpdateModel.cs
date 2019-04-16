using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Angelo.Connect.Models;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.UI.ViewModels.Validation;
using Angelo.Connect.Security;

namespace Angelo.Connect.Announcement.UI.ViewModels
{
    public class AnnouncementPostSettingsUpdateModel
    {
        public string AnnouncementPostId { get; set; }

        public bool IsPrivate { get; set; }

        public string CategoryIds { get; set; }
    }
}
