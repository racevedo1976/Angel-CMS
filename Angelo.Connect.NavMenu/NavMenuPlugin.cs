
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Icons;
using Angelo.Connect.NavMenu.Data;
using Angelo.Connect.NavMenu.Services;
using Angelo.Connect.NavMenu.UI.Components;
using Angelo.Connect.Widgets;
using Angelo.Connect.Widgets.Models;
using Angelo.Connect.Widgets.Services;
using Angelo.Plugins;
using Angelo.Connect.NavMenu.Models;

namespace Angelo.Connect.NavMenu
{
    public class NavMenuPlugin: IPlugin
    {
        public string Name { get; } = "Navigation Menu Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Plugin created during demo";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {

            pluginBuilder.ConfigureServices(services =>
            {
                services.AddTransient<AutoNavViewService>();
                services.AddTransient<NavMenuViewService>();
                services.AddTransient<IWidgetNamedModelProvider, NavMenuNamedModelProvider>();
            });

            pluginBuilder.AddDbContext<NavMenuDbContext>(db => {
                NavMenuDbActions.CreateSchemas(db);
                NavMenuDbActions.CreateTables(db);
                NavMenuDbActions.InsertSeedData(db);
            });

            pluginBuilder.RegisterWidget<NavMenuWidgetService, NavMenuWidget>(widget =>
            {
                widget.Category = "Menus";
                widget.WidgetType = "navmenu";
                widget.WidgetName = "Navigation Menu";
            })
            .AddForm<NavMenuWidgetConfigForm>(f =>
            {
                f.Title = "Settings";
                f.AjaxFlags = AjaxFlags.ALL;
            })
            .AddView(view =>
            {
                view.Id = "h-nav-menu";
                view.Title = "Horizontal Navigation";
                view.Path = "~/UI/Views/Widgets/HorizontalNavMenuWidget.cshtml";
                view.IconClass = "fa fa-bars";
            })
            .AddView(view =>
             {
                 view.Id = "v-nav-menu";
                 view.Title = "Vertical Navigation";
                 view.Path = "~/UI/Views/Widgets/VerticalNavMenuWidget.cshtml";
                 view.IconClass = "fa fa-bars";
             });


            // Auto Child Navigation Widget
            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(widget => {
                widget.Category = "Menus";
                widget.WidgetType = "navauto";
                widget.WidgetName = "Auto Child Navigation";
            })
            .AddView(view => {
                view.Id = "v-nav-auto";
                view.Title = "Child Page Navigation";
                view.Path = "~/UI/Views/Widgets/AutoNavWidget.cshtml";
                view.IconClass = "fa fa-bars";
            });

        }
    }
}
