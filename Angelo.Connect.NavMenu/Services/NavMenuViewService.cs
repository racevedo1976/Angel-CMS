using System.Collections.Generic;
using System.Threading.Tasks;

using Angelo.Connect.NavMenu.Data;
using Angelo.Connect.Data;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Kendo.Mvc.UI;
using Angelo.Common.Mvc;
using System.Linq;

namespace Angelo.Connect.NavMenu.Services
{
    public class NavMenuViewService
    {
        private NavMenuDbContext _navMenuDb;
        private ConnectDbContext _connectDb;
        private NavigationMenuManager _navMenuManager;

        public NavMenuViewService(NavMenuDbContext navMenuDb, ConnectDbContext connectDb, NavigationMenuManager navMenuManager)
        {
            _navMenuDb = navMenuDb;
            _connectDb = connectDb;
            _navMenuManager = navMenuManager;
        }

        private async Task LoadNodesIntoKendoMenuItemsAsync(ICollection<NavigationMenuItem> navMenuItems, IList<MenuItem> menuItems)
        {
            foreach (var item in navMenuItems)
            {
                var menuItem = new MenuItem();
                menuItem.Text = item.Title;
                menuItem.Url = _navMenuManager.GetNavMenuContentLink(item);
                await LoadNodesIntoKendoMenuItemsAsync(item.Children, menuItem.Items);
                menuItems.Add(menuItem);
            }
        }

        public async Task<List<MenuItem>> GetKendoMenuItemsAsync(string navMenuId)
        {
            var menuItems = new List<MenuItem>();
            var navMenu = await _navMenuManager.GetFullNavMenuAsync(navMenuId);
            if (navMenu != null)
                await LoadNodesIntoKendoMenuItemsAsync(navMenu.MenuItems, menuItems);
            return menuItems;
        }

        public async Task<List<NavMenuNode>> GetNavMenuNodesAsync(string navMenuId)
        {
            var menuNodes = new List<NavMenuNode>();
            var menuItems = await _navMenuManager.GetNavMenuItemsAsync(navMenuId);
            foreach (var item in menuItems)
            {
                if (await _navMenuManager.Authorize(item))
                {
                    var node = new NavMenuNode();
                    node.Id = item.Id;
                    node.ParentId = item.ParentId;
                    node.Title = item.Title;
                    node.Order = item.Order;
                    node.Link = _navMenuManager.GetNavMenuContentLink(item);
                    node.HasChildren = menuItems.Any(x => x.ParentId == item.Id);
                    menuNodes.Add(node);
                }
            }
            return menuNodes;
        }

        private async Task AddNavMenuItemToListAsync(NavigationMenuItem navItem, List<NavMenuViewItem> viewItems)
        {
            if (await _navMenuManager.Authorize(navItem))
            {
                var item = new NavMenuViewItem();
                item.Id = navItem.Id;
                item.Title = navItem.Title;
                item.Link = _navMenuManager.GetNavMenuContentLink(navItem);
                if (string.IsNullOrEmpty(item.Link))
                    item.Link = "#";
                foreach (var child in navItem.Children)
                    await AddNavMenuItemToListAsync(child, item.Children);
                if (navItem.TargetType == false)
                {
                    item.Target = "_self";
                }else
                {
                    item.Target = "_blank";
                }
                viewItems.Add(item);
            }
        }

        public async Task<List<NavMenuViewItem>> GetNavMenuViewItemsAsync(string navMenuId)
        {
            var viewItems = new List<NavMenuViewItem>();
            var navMenu = await _navMenuManager.GetFullNavMenuAsync(navMenuId);
            if (navMenu != null)
            {
                foreach (var item in navMenu.MenuItems)
                    await AddNavMenuItemToListAsync(item, viewItems);
            }
            return viewItems;
        }


    }
}
