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
    public class AnnouncementPostSettingsViewModel
    {
        public string AnnouncementPostId { get; set; }

        public bool IsPrivate { get; set; }

        public IEnumerable<AnnouncementCategory> Categories { get; set; }

        public IEnumerable<ContentVersion> Versions { get; set; }

        public IEnumerable<SecurityClaimConfig> PostPrivacyConfig { get; set; }
    }
}
