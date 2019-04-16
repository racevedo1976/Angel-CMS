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
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.Security;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.UI.Components;
using Angelo.Plugins;

namespace Angelo.Connect.News
{
    public class NewsPlugin : IPlugin
    {
        public string Name { get; } = "News Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Initial News Plugin";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services => {
                // register migrations
                services.AddTransient<IAppMigration, P910000_CreateInitialNewsTables>();
                services.AddTransient<IAppMigration, P910005_AddIsPrivateColumn>();
                services.AddTransient<IAppMigration, P910006_AddActiveColumn>();
                services.AddTransient<IAppMigration, P910015_AddPublishedColumn>();
                services.AddTransient<IAppMigration, P910020_AddVersioningColumns>();
                services.AddTransient<IAppMigration, P910025_InsertMissingVersionInfo>();

                // internal services
                services.AddTransient<NewsManager>();
                services.AddTransient<NewsQueryService>();
                services.AddTransient<NewsSecurityService>();

                // framework services
                services.AddTransient<ISecurityPermissionProvider, NewsClientPermissionProvider>();
                services.AddTransient<ISecurityPermissionProvider, NewsSitePermissionProvider>();
                services.AddTransient<IMenuItemProvider, ContentMenu>();
                services.AddTransient<IMenuItemProvider, OptionsMenu>();

                // userconsole snap-in
                services.AddTransient<IUserConsoleCustomComponent, NewsConsole>();
            });

            pluginBuilder.AddDbContext<NewsDbContext>();
           
            pluginBuilder.RegisterWidget<NewsWidgetService, NewsWidget>(widget => {
                widget.WidgetType = "News";
                widget.WidgetName = "News Posts";
                widget.Category = "Advanced";
            })
            .AddForm<NewsWidgetForm>(form => {
                form.Title = "Default Settings";
            })
            .AddForm<NewsWidgetCategoryBase>(form => {
                form.Title = "Categories";
            })
            .AddView(view => {
                view.Id = "news-posts-normal";
                view.Title = "News";
                view.Path = "~/UI/Views/Widgets/NewsPostsNormal.cshtml";             
                view.IconClass = IconType.Theme.ToString();
            });
        }
    }
}
