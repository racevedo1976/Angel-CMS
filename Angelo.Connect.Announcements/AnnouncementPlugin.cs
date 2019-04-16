using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Common.Migrations;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.UserConsole;
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.UI.Components;
using Angelo.Plugins;

namespace Angelo.Connect.Announcement
{
    public class AnnouncementPlugin : IPlugin
    {
        public string Name { get; } = "Announcement Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Initial Announcement Plugin";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services => {
                // register migrations
                services.AddTransient<IAppMigration, P310000_CreateInitialAnnouncementTables>();
                services.AddTransient<IAppMigration, P310005_AddIsPrivateColumn>();
                services.AddTransient<IAppMigration, P310006_AddActiveColumn>();
                services.AddTransient<IAppMigration, P310015_AddPublishedColumn>();
                services.AddTransient<IAppMigration, P310020_AddVersioningColumns>();
                services.AddTransient<IAppMigration, P310025_InsertMissingVersionInfo>();

                // internal services
                services.AddTransient<AnnouncementManager>();
                services.AddTransient<AnnouncementQueryService>();
                services.AddTransient<AnnouncementSecurityService>();

                // framework services
                services.AddTransient<ISecurityPermissionProvider, AnnouncementClientPermissionProvider>();
                services.AddTransient<ISecurityPermissionProvider, AnnouncementSitePermissionProvider>();
                services.AddTransient<IMenuItemProvider, ContentMenu>();
                services.AddTransient<IMenuItemProvider, OptionsMenu>();

                // userconsole snap-in
                services.AddTransient<IUserConsoleCustomComponent, AnnouncementConsole>();
            });

            pluginBuilder.AddDbContext<AnnouncementDbContext>();
           
            pluginBuilder.RegisterWidget<AnnouncementWidgetService, AnnouncementWidget>(widget => {
                widget.WidgetType = "announcement";
                widget.WidgetName = "Announcement Posts";
                widget.Category = "Advanced";
            })
            .AddForm<AnnouncementWidgetForm>(form => {
                form.Title = "Default Settings";
            })
            .AddForm<AnnouncementWidgetCategoryBase>(form => {
                form.Title = "Categories";
            })
            .AddView(view => {
                view.Id = "announcement-posts-normal";
                view.Title = "Announcement";
                view.Path = "~/UI/Views/Widgets/AnnouncementPostsNormal.cshtml";             
                view.IconClass = IconType.Announcement.ToString();
            });
        }
    }
}
