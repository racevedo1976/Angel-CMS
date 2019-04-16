using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Widgets;
using Angelo.Connect.NavMenu.Models;

namespace Angelo.Connect.NavMenu.Services
{
    public class NavMenuNamedModelProvider : IWidgetNamedModelProvider
    {
        private ConnectDbContext _connectDb;
        private IContextAccessor<SiteContext> _siteContextAccessor;

        // TODO: Fix circular dependency error when requesting NavigationMenuManager so don't have to use dbContext directly;
        public NavMenuNamedModelProvider(IContextAccessor<SiteContext> siteContextAccessor, ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
            _siteContextAccessor = siteContextAccessor;
        }

        public IWidgetModel GetModel(string widgetType, string modelName, Type modelType)
        {
            var siteContext = _siteContextAccessor.GetContext();

            if (siteContext?.SiteId == null)
                throw new Exception("SiteContext required");

            if(widgetType == "navmenu" && modelType == typeof(NavMenuWidget))
            {
                NavigationMenu navigationMenu = null;


                if(modelName == "main-menu")
                    navigationMenu = GetNavigationMenuByScope(siteContext.SiteId, "main");

                else if(modelName == "side-menu")
                    navigationMenu = GetNavigationMenuByScope(siteContext.SiteId, "side");


                if (navigationMenu != null)
                {
                    // Id will be set later by framework services
                    return new NavMenuWidget
                    {
                        Title = navigationMenu.Title,
                        NavMenuId = navigationMenu.Id.ToString()
                    };
                }         
            }

            // Return null if not found so that other model providers will have a chance to run
            return null;
        }

        private NavigationMenu GetNavigationMenuByScope(string siteId, string scope)
        {
            // Try to find menu with the corresponding scope
            var targetMenu = _connectDb.NavigationMenu.FirstOrDefault(x =>
                x.SiteId == siteId && x.Scope == scope
            );

            // Otherwise return any menu
            if(targetMenu == null)
                targetMenu = _connectDb.NavigationMenu.FirstOrDefault(x => x.SiteId == siteId);

            return targetMenu;
        }
    }
}
