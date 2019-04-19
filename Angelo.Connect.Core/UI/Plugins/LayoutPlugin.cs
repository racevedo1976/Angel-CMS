using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Plugins;
using Angelo.Connect.Widgets.Services;
using Angelo.Connect.Widgets.Models;

namespace Angelo.Connect.UI.Plugins
{
    public class LayoutPlugin //: IPlugin
    {
        public string Name { get; } = "Layout Plugin";
        public string Description { get; } = "Provides layout components to group, structure, and align other content components";
        public string Version { get; } = "1.0.0";
        public string Author { get; } = "MySites";

        public void Startup(PluginBuilder pluginBuilder)
        {

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-2";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
            })
           .AddView(v => {
               v.Title = "2 Column";
               v.Path = "~/UI/Views/Widgets/Layout-2.cshtml";
               v.IconClass = "fa fa-columns";
           });

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-3";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
            })
           .AddView(v => {
               v.Title = "3 Column";
               v.Path = "~/UI/Views/Widgets/Layout-3.cshtml";
               v.IconClass = "fa fa-columns";
           });

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-4";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
                builder.IconUrl = "";
            })
           .AddView(v => {
               v.Title = "4 Column";
               v.Path = "~/UI/Views/Widgets/Layout-4.cshtml";
               v.IconClass = "fa fa-columns";
           });

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-6";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
                builder.IconUrl = "";
            })
           .AddView(v => {
               v.Title = "6 Column";
               v.Path = "~/UI/Views/Widgets/Layout-6.cshtml";
               v.IconClass = "fa fa-columns";
           });

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-3-9";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
                builder.IconUrl = "";
            })
            .AddView(v => {
                v.Title = "Split 25% / 75%";
                v.Path = "~/UI/Views/Widgets/LayoutSplit-3-9.cshtml";
                v.IconClass = "fa fa-th-list";
            });

            pluginBuilder.RegisterWidget<StaticWidgetService, StaticWidget>(builder => {
                builder.WidgetType = "layout-8-4";
                builder.WidgetName = "Content Layout";
                builder.Category = "Layout";
                builder.IconUrl = "";
            })
           .AddView(v => {
                v.Title = "Split 66% / 33% ";
                v.Path = "~/UI/Views/Widgets/LayoutSplit-8-4.cshtml";
                v.IconClass = "fa fa-table";
            });


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
                v.Path = "~/UI/Views/Widgets/Layout-Dropdown.cshtml";
                v.IconClass = "fa fa-caret-square-o-down";
            })
            .AddView(v =>
            {
                v.Id = "zone-tabs";
                v.Title = "Tabstrip";
                v.Path = "~/UI/Views/Widgets/Layout-Tabs.cshtml";
                v.IconClass = "fa fa-id-card-o";
            });
            */
        }

    }
}
