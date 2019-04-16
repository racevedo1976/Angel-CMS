using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

using Angelo.Common.Mvc;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.News.Security;

namespace Angelo.Connect.News
{
    public class ContentMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserContent;

        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public ContentMenu(NewsSecurityService newsSecurity, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var returnUrl = httpContext.Request.GetRelativeUrlEncoded();

            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "News Post",
                    Url = "javascript: void $.console('news', '/sys/console/News/posts/create')",
                    Icon = IconType.Theme,
                    AuthorizeCallback = user => {
                        return newsSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}
