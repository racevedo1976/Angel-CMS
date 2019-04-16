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
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.UI.Components;
using Angelo.Plugins;

namespace Angelo.Connect.Blog
{
    public class BlogPlugin : IPlugin
    {
        public string Name { get; } = "Blog Plugin";
        public string Version { get; } = "0.0.1";
        public string Description { get; } = "Initial Blog Plugin";
        public string Author { get; } = "SchoolInSites";

        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services => {
                // register migrations
                services.AddTransient<IAppMigration, P410000_CreateInitialBlogTables>();
                services.AddTransient<IAppMigration, P410005_AddIsPrivateColumn>();
                services.AddTransient<IAppMigration, P410006_AddActiveColumn>();
                services.AddTransient<IAppMigration, P410015_AddPublishedColumn>();
                services.AddTransient<IAppMigration, P410020_AddVersioningColumns>();
                services.AddTransient<IAppMigration, P410025_InsertMissingVersionInfo>();

                // internal services
                services.AddTransient<BlogManager>();
                services.AddTransient<BlogQueryService>();
                services.AddTransient<BlogSecurityService>();

                // framework services
                services.AddTransient<ISecurityPermissionProvider, BlogClientPermissionProvider>();
                services.AddTransient<ISecurityPermissionProvider, BlogSitePermissionProvider>();
                services.AddTransient<IMenuItemProvider, ContentMenu>();
                services.AddTransient<IMenuItemProvider, OptionsMenu>();

                // userconsole snap-in
                services.AddTransient<IUserConsoleCustomComponent, BlogConsole>();
            });

            pluginBuilder.AddDbContext<BlogDbContext>();
           
            pluginBuilder.RegisterWidget<BlogWidgetService, BlogWidget>(widget => {
                widget.WidgetType = "blog";
                widget.WidgetName = "Blog Posts";
                widget.Category = "Advanced";
            })
            .AddForm<BlogWidgetForm>(form => {
                form.Title = "Default Settings";
            })
            .AddForm<BlogWidgetCategoryBase>(form => {
                form.Title = "Categories";
            })
            .AddView(view => {
                view.Id = "blog-posts-normal";
                view.Title = "Blog";
                view.Path = "~/UI/Views/Widgets/PostsNormal.cshtml";             
                view.IconClass = IconType.List2.ToString();
            });
        }
    }
}
