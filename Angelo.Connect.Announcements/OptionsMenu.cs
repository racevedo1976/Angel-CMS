using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;

using Angelo.Common.Mvc;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.UI;
using Angelo.Connect.Security;
using Angelo.Connect.Announcement.Security;

namespace Angelo.Connect.Announcement
{
    public class OptionsMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserOptions;

        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public OptionsMenu(AnnouncementSecurityService announcementSecurity)
        {
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "Manage My Announcements",
                    Url = "javascript: void $.console('announcement')",
                    Icon = IconType.Announcement,
                    SortOrder = 10,
                    AuthorizeCallback = user => {
                        return announcementSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}
