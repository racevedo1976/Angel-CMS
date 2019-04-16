using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.Models;

namespace Angelo.Connect.Video
{
    public class ClientMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.ClientTools;

        public IEnumerable<IMenuItem> MenuItems { get; set; }


        public ClientMenu(IContextAccessor<SiteContext> siteContextAccessor)
        {
            var siteContext = siteContextAccessor.GetContext();

            MenuItems = new List<IMenuItem>()
            {
                // TODO: Add user security.
                new MenuItemSecureCustom()
                {
                    Icon = IconType.VideoCamera,
                    Title = "Live Streams",
                    Url = "/Admin/VideoStreamLink",
                    AuthorizeCallback = enabled =>
                    {
                        return siteContext.ProductContext.Features.Get(FeatureId.LiveVideo)?.GetSettingValue<bool>("enabled") ?? false;
                    }
                }
            };
        }
    }
}
