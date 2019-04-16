using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;

namespace Angelo.Connect.Menus
{
    public class LibraryMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.LibraryTools;

        public IEnumerable<IMenuItem> MenuItems { get; set; }
       
        public LibraryMenu()
        {
            BuildMenuItems();
        }

        private void BuildMenuItems()
        {
            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureClaims() {
                    Title = "My Root", Url = "/UI/Views/Admin/User/Documents.cshtml", Icon = IconType.Briefcase, Active = true
                    //,Children = new List<IMenuItem>()
                    //{
                    //    new MenuItem { Title = "Link.Search", Icon = IconType.Search, Url = "/admin/clients/index" },
                    //    new MenuItem { Title = "Link.Create", Icon = IconType.Add, Url = "/admin/clients/create" },
                    //}
                },
                new MenuItemSecureClaims() {
                    Title = "Trash", Url = "/admin/products/clientproducts", Icon = IconType.Trashcan
                    //,Children = new List<IMenuItem>()
                    //{
                    //    new MenuItem() { Title = "Link.ProductXml", Url = "/admin/products/clientproducts", Icon = IconType.Download },
                    //    new MenuItem() { Title = "Link.OpenId", Url = "/admin/dashboard/aegisinfo", Icon = IconType.Fire },
                    //}
                },
                new MenuItemSecureClaims() { Title = "Share", Url = "/admin/jobs/index", Icon = IconType.Collections },
            };
        }
    }
}
