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
    public class ContentMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserContent;

        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public ContentMenu(AnnouncementSecurityService announcementSecurity, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var returnUrl = httpContext.Request.GetRelativeUrlEncoded();

            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "Announcement Post",
                    Url = "javascript: void $.console('announcement', '/sys/console/announcement/posts/create')",
                    Icon = IconType.Announcement,
                    AuthorizeCallback = user => {
                        return announcementSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}
