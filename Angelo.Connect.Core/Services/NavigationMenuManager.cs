using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Angelo.Connect.Services
{
    public class NavigationMenuManager
    {
        private ConnectDbContext _db;
        private IEnumerable<INavMenuContentProvider> _navMenuContentProviders;

        public NavigationMenuManager(ConnectDbContext dbContext, IEnumerable<INavMenuContentProvider> navMenuContentProviders)
        {
            _db = dbContext;
            _navMenuContentProviders = navMenuContentProviders;
        }

        public INavMenuContentProvider GetNavMenuContentProvider(string name)
        {
            return _navMenuContentProviders.FirstOrDefault(x => x.Name == name);
        }

        public INavMenuContentProvider GetNavMenuContentProviderOfContentId(string contentId)
        {
            foreach(var provider in _navMenuContentProviders)
                if (provider.GetContentItem(contentId) != null)
                    return provider;
            return null;
        }

        public IEnumerable<INavMenuContentProvider> GetNavMenuContentProviders()
        {
            return _navMenuContentProviders;
        }

        public string GetNavMenuContentLink(NavigationMenuItem item)
        {
            if (item.ContentType == NavigationMenuItemType.Label)
                return null;

            if (item.ContentType == NavigationMenuItemType.ExternalURL)
                return item.ExternalURL;

            var provider = GetNavMenuContentProvider(item.ContentType);
            if (provider == null)
                return null;
            var content = provider.GetContentItem(item.ContentId);
            if (content == null)
                return null;
            return content.Link;
        }

        public async Task<bool> Authorize(NavigationMenuItem item)
        {
            if (item.ContentType == NavigationMenuItemType.Label)
                return true;

            if (item.ContentType == NavigationMenuItemType.ExternalURL)
                return true;

            var provider = GetNavMenuContentProvider(item.ContentType);
            if (provider == null)
                throw new Exception("No provider");

            return await provider.Authorize(item.ContentId);
        }

        public IQueryable<NavigationMenu> GetNavMenusOfSiteIdQuery(string siteId)
        {
            Ensure.Argument.NotNull(siteId);

            return _db.NavigationMenu.AsNoTracking().Where(x => x.SiteId == siteId);
        }

        public async Task<ICollection<NavigationMenu>> GetNavMenusOfSiteIdAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId);

            return await GetNavMenusOfSiteIdQuery(siteId).ToListAsync();
        }

        public async Task<NavigationMenu> GetNavMenuAsync(string navMenuId)
        {
            return await _db.NavigationMenu.AsNoTracking()
                .Where(x => x.Id == navMenuId)
                .FirstOrDefaultAsync();
        }

        protected void LoadChildrenOfMenuItem(NavigationMenuItem item, List<NavigationMenuItem> items)
        {
            var children = items.Where(x => x.ParentId == item.Id).OrderBy(x => x.Order).ToList();
            foreach(var child in children)
            {
                LoadChildrenOfMenuItem(child, items);
                item.Children.Add(child);
            }
        }

        public async Task<NavigationMenu> GetFullNavMenuAsync(string navMenuId)
        {
            var navMenu = await GetNavMenuAsync(navMenuId);
            if (navMenu != null)
            {
                var menuItems = await GetNavMenuItemsAsync(navMenuId);
                var rootItems = menuItems.Where(x => x.ParentId == null).OrderBy(x => x.Order).ToList();
                foreach(var item in rootItems)
                {
                    LoadChildrenOfMenuItem(item, menuItems);
                    navMenu.MenuItems.Add(item);
                }
            }
            return navMenu;
        }

        public async Task<List<NavigationMenuItem>> GetNavMenuItemsAsync(string navMenuId)
        {
            return await _db.NavigationMenuItems.AsNoTracking()
                .Where(x => x.NavMenuId == navMenuId)
                .ToListAsync();
        }

        public async Task<NavigationMenuItem> GetNavMenuItemAsync(string navMenuItemId)
        {
            Ensure.Argument.NotNullOrEmpty(navMenuItemId);

            return await _db.NavigationMenuItems.AsNoTracking()
                .Where(x => x.Id == navMenuItemId)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteNavMenuAsync(string navMenuId)
        {
            Ensure.Argument.NotNull(navMenuId);

            var menu = await _db.NavigationMenu.FirstOrDefaultAsync(x => x.Id == navMenuId);
            if (menu != null)
            {
                var items = await _db.NavigationMenuItems.Where(x => x.NavMenuId == navMenuId).ToListAsync();
                foreach (var item in items)
                    _db.NavigationMenuItems.Remove(item);
                _db.NavigationMenu.Remove(menu);
                await _db.SaveChangesAsync();
            }
        }

        private async Task DeleteMenuItemChildrenAsync(NavigationMenuItem item)
        {
            var children = await _db.NavigationMenuItems.Where(x => x.ParentId == item.Id).ToListAsync();
            foreach(var child in children)
            {
                await DeleteMenuItemChildrenAsync(child);
                _db.NavigationMenuItems.Remove(child);
            }
        }

        public async Task DeleteMenuItemAsync(string menuItemId)
        {
            Ensure.Argument.NotNullOrEmpty(menuItemId);

            var item = await _db.NavigationMenuItems.FirstOrDefaultAsync(x => x.Id == menuItemId);
            if (item != null)
            {
                var children = await _db.NavigationMenuItems
                    .Where(x => x.NavMenuId == item.NavMenuId && x.ParentId == item.ParentId)
                    .OrderBy(x => x.Order)
                    .ToListAsync();

                var count = 0;
                foreach (var child in children)
                {
                    if (child.Id != item.Id)
                    {
                        count = count + 1;
                        child.Order = count;
                    }
                }
                await DeleteMenuItemChildrenAsync(item);
                _db.NavigationMenuItems.Remove(item);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateNavMenuAsync(NavigationMenu navMenu)
        {
            Ensure.Argument.NotNull(navMenu);

            var menu = await _db.NavigationMenu.FirstOrDefaultAsync(x => x.Id == navMenu.Id);
            if (menu != null)
            {
                menu.Title = navMenu.Title;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<NavigationMenu> InsertNavMenuAsync(NavigationMenu navMenu)
        {
            var menu = new NavigationMenu();
            menu.Id = Guid.NewGuid().ToString("N");
            menu.SiteId = navMenu.SiteId;
            menu.Title = navMenu.Title;
            _db.NavigationMenu.Add(menu);
            await _db.SaveChangesAsync();
            return menu;
        }

        public async Task<NavigationMenu> SaveNavMenuAsync(NavigationMenu navMenu)
        {
            if (await _db.NavigationMenu.AnyAsync(x => x.Id == navMenu.Id))
            {
                await UpdateNavMenuAsync(navMenu);
                return navMenu;
            }
            else
            {
                return await InsertNavMenuAsync(navMenu);
            }
        }

        public async Task UpdateNavMenuItemAsync(NavigationMenuItem item)
        {
            Ensure.Argument.NotNull(item);

            var dbItem = await _db.NavigationMenuItems.FirstOrDefaultAsync(x => x.Id == item.Id);
            Ensure.NotNull(dbItem, $"Unable to find NavigationMenuItem (Id={item.Id})");

            dbItem.Title = item.Title;
            dbItem.ExternalURL = item.ExternalURL;
            dbItem.ContentType = item.ContentType;
            dbItem.ContentId = item.ContentId;
            dbItem.TargetType = item.TargetType;
            await _db.SaveChangesAsync();
        }

        public async Task<NavigationMenuItem> InsertNavMenuItemAsync(NavigationMenuItem item)
        {
            Ensure.Argument.NotNull(item);

            var itemCount = await _db.NavigationMenuItems
                .Where(x => (x.ParentId == item.ParentId) && (x.NavMenuId == item.NavMenuId))
                .CountAsync();
            var newItem = new NavigationMenuItem();
            newItem.Id = Guid.NewGuid().ToString("N");
            newItem.NavMenuId = item.NavMenuId;
            newItem.ParentId = item.ParentId;
            newItem.Title = item.Title;
            newItem.ExternalURL = item.ExternalURL;
            newItem.ContentType = item.ContentType;
            newItem.ContentId = item.ContentId;
            newItem.TargetType = item.TargetType;
            newItem.Order = itemCount + 1;
            _db.NavigationMenuItems.Add(newItem);
            await _db.SaveChangesAsync();
            return newItem;
        }

        public async Task MoveNavMenuItemRelativeToDestItem(string sourceItemId, string destItemId, string dropPosition)
        {
            if (sourceItemId == destItemId)
                return;

            var destItem = await GetNavMenuItemAsync(destItemId);
            Ensure.NotNull(destItem, $"Unable to find NavigationMenuItem: [Id = {sourceItemId}]");

            var menuId = destItem.NavMenuId;

            // Get all menu items in the same dropped location
            var correctedTargetMenuLevel = ((dropPosition == "over") ? destItem.Id : destItem.ParentId);
            var children = await _db.NavigationMenuItems
                .Where(x => x.ParentId == correctedTargetMenuLevel && x.Id != sourceItemId && x.NavMenuId == menuId)
                .OrderBy(x => x.Order)
                .ToListAsync();

            //Get menu item on the move
            var menuItemToMove = await _db.NavigationMenuItems.FirstOrDefaultAsync(x => x.Id == sourceItemId);
            if (menuItemToMove == null)
                throw new Exception($"Unable to find NavigationMenuItem: [Id: {menuItemToMove}]");

            var itemOriginalParentId = menuItemToMove.ParentId;
            string toParentId = correctedTargetMenuLevel;
            var lastPlaceInNewParentId = children.Count() + 1;

            //If moving within the same level, 
            //sort the target level items to start off with normalized list without the menu item moved.
            if(itemOriginalParentId == toParentId)
            {
                if (children.Any())
                {
                    var newIndex = 1;
                    foreach (var menuItemFutureSibling in children)
                    {
                        menuItemFutureSibling.Order = newIndex;
                        newIndex++;
                    }
                }
            }
            
            //update the moving menu item parent id
            menuItemToMove.ParentId = toParentId;

            //figure out the new Order # for the moving item
            switch (dropPosition)
            {
                case "over":
                    menuItemToMove.Order = lastPlaceInNewParentId;
                    
                    break;

                case "after":
                    menuItemToMove.Order = children.FirstOrDefault(x => x.Id == destItem.Id).Order + 1;
                    
                    break;

                case "before":
                    var currentDestOrder = children.FirstOrDefault(x => x.Id == destItem.Id).Order;
                    menuItemToMove.Order = currentDestOrder == 1 ? 1 : currentDestOrder - 1;

                    break;
                default:
                    throw new Exception("Unknown dropPosition value: " + (dropPosition ?? "NULL"));
            }

            //reorder and insert the moving menu item in to its new place level
            if (children.Any())
            {
                int index = 1;
                foreach (var menuItem in children)
                {
                    if (index == menuItemToMove.Order)
                        index++;

                    menuItem.Order = index;
                    index++;
                }
            }

            //reorder the original location where menu item is moving from IF its different parent, meaning different level location
            if (itemOriginalParentId != toParentId)
            {
                // Get all menu items where item came from 
                var itemsInOriginalLevel = await _db.NavigationMenuItems
                    .Where(x => x.ParentId == itemOriginalParentId && x.Id != sourceItemId && x.NavMenuId == menuId)
                    .OrderBy(x => x.Order)
                    .ToListAsync();

                if (itemsInOriginalLevel.Any())
                {
                    var newIndex = 1;
                    foreach (var prevSiblingMenuItem in itemsInOriginalLevel)
                    {
                        prevSiblingMenuItem.Order = newIndex;
                        newIndex++;
                    }
                }
            }

            await _db.SaveChangesAsync();

        }

      
    }
}
