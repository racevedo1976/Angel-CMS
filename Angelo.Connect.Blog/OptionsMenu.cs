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
using Angelo.Connect.Blog.Security;

namespace Angelo.Connect.Blog
{
    public class OptionsMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserOptions;

        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public OptionsMenu(BlogSecurityService blogSecurity)
        {
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "Manage My Blogs",
                    Url = "javascript: void $.console('blog')",
                    Icon = IconType.File,
                    SortOrder = 10,
                    AuthorizeCallback = user => {
                        return blogSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}
