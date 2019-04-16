using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Angelo.Common.Migrations;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.Widgets;
using Angelo.Connect.Video.Data;
using Angelo.Connect.Video.Models;
using Angelo.Connect.Video.Services;
using Angelo.Connect.Video.UI.Components;
using Angelo.Plugins;

namespace Angelo.Connect.Video
{
    public class VideoPlugin: IPlugin
    {
        public string Name { get; } = "Video Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Plugin created during demo";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {

            pluginBuilder.ConfigureServices(services => {
                // register migrations
                services.AddTransient<IAppMigration, P710000_CreateInitialVideoTables>();
                services.AddTransient<IAppMigration, P710005_CreateVideoBackgroundTable>();

                // internal services
                services.AddTransient<VideoStreamLinkService>();

                // framework services
                services.AddTransient<IMenuItemProvider, ClientMenu>();
            });

            pluginBuilder.AddDbContext<VideoDbContext>();

            //-- Video Player Widget --
            pluginBuilder.RegisterWidget<VideoWidgetService, VideoWidgetViewModel>(widget =>
            {
                widget.Category = "Media";
                widget.WidgetType = "video";
                widget.WidgetName = "Video Player";
            })
            .AddForm<VideoWidgetConfigForm>(f =>
            {
                f.Title = "General Settings";
                f.AjaxFlags = AjaxFlags.ALL;
            })
            .AddForm<VideoWidgetSummaryForm>(f =>
            {
                f.Title = "Serialization";
            })
            .AddView(view =>
            {
                view.Id = "video-player";
                view.Title = "Video";
                view.Path = "~/UI/Views/Widgets/VideoPlayer.cshtml";
                view.IconClass = IconType.VideoCamera.ToString();
            });

            //-- Video Background Widget --
            pluginBuilder.RegisterWidget<VideoBackgroundWidgetService, VideoBackgroundWidgetViewModel>(widget => {
                widget.Category = "Media";
                widget.WidgetType = "video-background";
                widget.WidgetName = "Video Background";
            })
            .AddForm<VideoBackgroundWidgetConfigForm>(f => {
                f.Title = "Default Settings";
            })
            .AddView(v => {
                v.Id = "video-background";
                v.Title = "Video Background";
                v.Path = "~/UI/Views/Widgets/VideoBackground.cshtml";
                v.IconClass = IconType.VideoCamera.ToString();
            });

        }
    }
}
