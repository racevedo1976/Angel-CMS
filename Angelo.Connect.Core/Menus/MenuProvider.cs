using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Common.Extensions;

namespace Angelo.Connect.Menus
{
    public class MenuProvider
    {
        private IEnumerable<IMenuItemProvider> _registeredMenus;
        private IContextAccessor<AdminContext> _adminContextAccessor;
         
        public MenuProvider(IEnumerable<IMenuItemProvider> menus, IContextAccessor<AdminContext> adminContextAccessor)
        {
            _registeredMenus = menus;
            _adminContextAccessor = adminContextAccessor;
        }

        /// <summary>
        /// Gets authorized menu items for a single TMenuItemProvider
        /// </summary>
        public async Task<IEnumerable<IMenuItem>> GetMenuItemsAsync<TMenuItemProvider>() where TMenuItemProvider : IMenuItemProvider
        {
            foreach(var menu in _registeredMenus)
            {
                if(menu.GetType() == typeof(TMenuItemProvider))
                {
                    return await GetAuthorizedMenuItems(new IMenuItemProvider[] { menu });
                }
            }

            return new IMenuItem[] { };
        }

        /// <summary>
        /// Gets authorized menus items for all IMenuItemProviders corresponding to the MenuType specified
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IMenuItem>> GetMenuItemsAsync(string menuType)
        {
            var selectedMenus = _registeredMenus.Where(x => x.MenuName == menuType);

            return await GetAuthorizedMenuItems(selectedMenus);
        }

        private async Task<IEnumerable<IMenuItem>> GetAuthorizedMenuItems(IEnumerable<IMenuItemProvider> menuItemProviders)
        {
            var authorizedMenuItems = new List<IMenuItem>();
            var adminContext = _adminContextAccessor.GetContext();
            var user = adminContext.UserContext;

            foreach (var menu in menuItemProviders)
            {
                var menuItems = GetAuthorizedChildItems(menu.MenuItems, user).ToList();

                if (menuItems != null && menuItems.Count > 0)
                {
                    authorizedMenuItems.AddRange(menuItems);
                }
            }

            return await Task.FromResult(authorizedMenuItems);
        }

        private IEnumerable<IMenuItem> GetAuthorizedChildItems(IEnumerable<IMenuItem> menuItems, UserContext user)
        {
            var authItems = new List<IMenuItem>();

            foreach(var item in menuItems)
            {
                if(item.Authorize(user))
                {
                    authItems.Add(item);
                    item.MenuItems = GetAuthorizedChildItems(item.MenuItems, user).ToList();
                }
            }

            return authItems;
        }
    }
}
