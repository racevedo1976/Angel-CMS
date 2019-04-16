using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.UserConsole;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.Announcement
{
    public class AnnouncementConsole : IUserConsoleCustomComponent
    {
        private AnnouncementManager _announcementManager;
        private AnnouncementQueryService _announcementQueries;
        private AnnouncementSecurityService _announcementSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public int ComponentOrder { get; } = 120;

        public string ComponentType { get; } = "announcement";

        public string InitialRoute { get; } = "/sys/console/announcement/posts";

        public AnnouncementConsole
        (
          AnnouncementManager announcementManager,
          AnnouncementQueryService announcementQueries,
          AnnouncementSecurityService announcementSecurity,
          IContextAccessor<UserContext> userContextAccessor
        )
        {
            _announcementManager = announcementManager;
            _announcementQueries = announcementQueries;
            _announcementSecurity = announcementSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public async Task<GenericViewResult> ComposeExplorer()
        {
            var user = _userContextAccessor.GetContext();
            var categories = _announcementManager.GetAnnouncementCategoriesOwnedByUser(user.UserId);

            categories = categories.OrderBy(x => x.Title).ToList();

            var result = new GenericViewResult
            {
                Title = "My Announcements",
                ViewPath = "/UI/Views/Console/Announcements/AnnouncementExplorer.cshtml",
                ViewModel = categories
            };

            return await Task.FromResult(result);
        }
    }
}
