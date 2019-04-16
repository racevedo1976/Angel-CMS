using System.Collections.Generic;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.News.Security;

namespace Angelo.Connect.News
{
    public class OptionsMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserOptions;

        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public OptionsMenu(NewsSecurityService newsSecurity)
        {
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "Manage My News",
                    Url = "javascript: void $.console('news')",
                    Icon = IconType.Theme,
                    SortOrder = 10,
                    AuthorizeCallback = user => {
                        return newsSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}
