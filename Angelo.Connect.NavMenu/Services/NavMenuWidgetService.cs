using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.NavMenu.Data;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Connect.NavMenu.Models;

namespace Angelo.Connect.NavMenu.Services
{
    public class NavMenuWidgetService : IWidgetService<NavMenuWidget>
    {
        private NavMenuDbContext _navMenuDb;
        private ConnectDbContext _connectDb;

        public NavMenuWidgetService(NavMenuDbContext navMenuDb, ConnectDbContext connectDb)
        {
            _navMenuDb = navMenuDb;
            _connectDb = connectDb;
        }

        public void DeleteModel(string widgetId)
        {
            var widget = _navMenuDb.NavMenuWidgets.FirstOrDefault(x => x.Id == widgetId);
            _navMenuDb.NavMenuWidgets.Remove(widget);
            _navMenuDb.SaveChanges();
        }

        public NavMenuWidget GetDefaultModel()
        {
            var model = new NavMenuWidget()
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Navigation Menu",
                NavMenuId = string.Empty
            };
            return model;
        }

        public NavMenuWidget GetModel(string widgetId)
        {
            var widget = _navMenuDb.NavMenuWidgets.AsNoTracking().FirstOrDefault(x => x.Id == widgetId);
            if (widget == null)
                return null;
            else
                return widget;
        }

        public void SaveModel(NavMenuWidget model)
        {
            _navMenuDb.NavMenuWidgets.Add(model);
            _navMenuDb.SaveChanges();
        }

        public void UpdateModel(NavMenuWidget model)
        {
            _navMenuDb.Attach(model);
            _navMenuDb.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _navMenuDb.SaveChanges();
        }

        public NavMenuWidget CloneModel(NavMenuWidget model)
        {
            var widget = new NavMenuWidget()
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = model.Title,
                NavMenuId = model.NavMenuId
            };
            _navMenuDb.NavMenuWidgets.Add(widget);
            _navMenuDb.SaveChanges();
            return widget;
        }
    }
}
