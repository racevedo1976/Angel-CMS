using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.CoreWidgets.Data;
using AutoMapper.Extensions;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class NavBarService : IWidgetService<NavBar>
    {
        private HtmlDbContext _htmlDb;
        private ConnectDbContext _connectDb;

        public NavBarService(HtmlDbContext htmlDb, ConnectDbContext connectDb)
        {
            _htmlDb = htmlDb;
            _connectDb = connectDb;
        }

        public void SaveModel(NavBar model)
        {
            _htmlDb.NavBars.Add(model);
            _htmlDb.SaveChanges();
        }

        public void UpdateModel(NavBar model)
        {
            _htmlDb.Attach<NavBar>(model);
            _htmlDb.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _htmlDb.SaveChanges();
        }

        public NavBar CloneModel(NavBar model)
        {
            var cloned = model.Clone();

            cloned.Id = Guid.NewGuid().ToString("N");

            _htmlDb.NavBars.Add(cloned);
            _htmlDb.SaveChanges();

            return cloned;
        }

        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _htmlDb.NavBars.Remove(model);
            _htmlDb.SaveChanges();
        }

        public NavBar GetModel(string widgetId)
        {
            var navBar = _htmlDb.NavBars.FirstOrDefault(x => x.Id == widgetId);

            if(navBar?.NavMenuId != null)
            {
                navBar.NavMenu = _connectDb.NavigationMenu
                    .Include(x => x.MenuItems)
                    .FirstOrDefault(x => x.Id == navBar.NavMenuId);

                foreach(var item in navBar.NavMenu.MenuItems)
                {
                    if (item.ExternalURL == null) item.ExternalURL = "";
                    item.NavMenu = null;
                }
            }
            else
            {
                var defaultModel = GetDefaultModel();
                navBar.NavMenu = defaultModel.NavMenu;
            }

            return navBar;
        }

        public NavBar GetDefaultModel()
        {
            return new NavBar
            {
                ItemWidth = "100px",
                NavMenu = new NavigationMenu
                {
                    Title = "Demo Menu",
                    MenuItems = new List<NavigationMenuItem>
                    {
                        new NavigationMenuItem { Title = "Home", ExternalURL = "" },
                        new NavigationMenuItem { Title = "About", ExternalURL = "about" },
                        new NavigationMenuItem { Title = "Contact", ExternalURL = "contact" },
                    }
                }
            };
        }
    }
}
