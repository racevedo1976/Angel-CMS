using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Icons;
using Angelo.Plugins;
using Angelo.Connect.Widgets.Services;
using Angelo.Connect.Widgets.Models;
using Angelo.Connect.Menus;

namespace Angelo.Connect.UI.Plugins
{
    public class ZonePlugin : IPlugin
    {
        public string Name { get; } = "Zone Plugin";
        public string Description { get; } = "Provides layout components to group, structure, and align other content components";
        public string Version { get; } = "1.0.0";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "zone";
                builder.WidgetName = "Content Zone";
                builder.Category = "Layout";
                builder.IconUrl = "";
            })
            .AddView(v => {
                v.Id = "zone-1";
                v.Title = "1 Column";
                v.Path = "~/UI/Views/Zones/Zone-1.cshtml";
                v.IconClass = IconType.Column1.ToString();
            })
            .AddView(v => {
                v.Id = "zone-2";
                v.Title = "2 Column";
                v.Path = "~/UI/Views/Zones/Zone-2.cshtml";
                v.IconClass = IconType.Column2.ToString();
            })
            .AddView(v => {
                v.Id = "zone-3";
                v.Title = "3 Column";
                v.Path = "~/UI/Views/Zones/Zone-3.cshtml";
                v.IconClass = IconType.Column3.ToString();
            })
            .AddView(v => {
                v.Id = "zone-4";
                v.Title = "4 Column";
                v.Path = "~/UI/Views/Zones/Zone-4.cshtml";
                v.IconClass = IconType.Column4.ToString();
            })
            .AddView(v => {
                v.Id = "zone-5";
                v.Title = "5 Column";
                v.Path = "~/UI/Views/Zones/Zone-5.cshtml";
                v.IconClass = IconType.Column5.ToString();
            })
            .AddView(v => {
                v.Id = "zone-6";
                v.Title = "6 Column";
                v.Path = "~/UI/Views/Zones/Zone-6.cshtml";
                v.IconClass = IconType.Column6.ToString();
            })
            .AddView(v => {
                v.Id = "zone-2-10";
                v.Title = "20/80% Column";
                v.Path = "~/UI/Views/Zones/Zone-2-10.cshtml";
                v.IconClass = IconType.Column80Pct.ToString();
            })
            .AddView(v => {
                 v.Id = "zone-3-9";
                 v.Title = "25/75% Column";
                 v.Path = "~/UI/Views/Zones/Zone-3-9.cshtml";
                 v.IconClass = IconType.Column75Pct.ToString();
             })
            .AddView(v => {
                v.Id = "zone-9-3";
                v.Title = "75/25% Column";
                v.Path = "~/UI/Views/Zones/Zone-9-3.cshtml";
                v.IconClass = IconType.Column25Pct.ToString();
            })
            
            .AddView(v => {
                v.Id = "zone-4-8";
                v.Title = "33/66% Column";
                v.Path = "~/UI/Views/Zones/Zone-4-8.cshtml";
                v.IconClass = IconType.Column66Pct.ToString();
            })
            .AddView(v => {
                v.Id = "zone-8-4";
                v.Title = "66/33% Column";
                v.Path = "~/UI/Views/Zones/Zone-8-4.cshtml";
                v.IconClass = IconType.Column33Pct.ToString();
            });

            


            // Old Test Widgets
            /*
            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder =>
            {
                builder.WidgetType = "menu";
                builder.WidgetName = "Content Menu";
                builder.Category = "Menus";
                builder.IconUrl = "";
            }).AddView(v =>
            {
                v.Id = "zone-dropdown";
                v.Title = "Dropdown";
                v.Path = "~/UI/Views/Zones/Zone-Dropdown.cshtml";
                v.IconClass = "fa fa-caret-square-o-down";
            })
            .AddView(v =>
            {
                v.Id = "zone-tabs";
                v.Title = "Tabstrip";
                v.Path = "~/UI/Views/Zones/Zone-Tabs.cshtml";
                v.IconClass = "fa fa-id-card-o";
            });
            */
        }

    }
}
