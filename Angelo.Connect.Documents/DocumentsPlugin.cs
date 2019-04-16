using Angelo.Common.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;


using Angelo.Connect.Documents.Services;
using Angelo.Connect.Documents.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.Documents.Data;
using Angelo.Connect.Icons;
using Angelo.Connect.Documents.UI.Components;
using Angelo.Connect.Data;
using Angelo.Connect.Documents.Data.Migrations;
using Angelo.Connect.Widgets;
using Angelo.Plugins;

namespace Angelo.Connect.Documents
{
    public class DocumentsPlugin : IPlugin
    {
        public DocumentsPlugin()
        {

        }

        public string Author { get; } = "SchoolInSites";
        public string Description { get; } = "Documents Plugin";
        public string Name { get; } = "Documents Plugin";
        public string Version { get; } = "0.0.1";
        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services =>
            {
                var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();

                services.AddTransient<DocumentListService>();

                // register migrations
                services.AddTransient<IAppMigration, P510000_CreateInitialDocumentTables>();
                services.AddTransient<IAppMigration, P510001_AddFolderTableAndSortField>();
            });

            pluginBuilder.AddDbContext<DocumentListDbContext>();
 
            pluginBuilder.RegisterWidget<DocumentListWidgetService, DocumentListWidget>(widget =>
            {
                widget.WidgetType = "document";
                widget.WidgetName = "Document List";
                widget.Category = "Media";
            })
           
            .AddForm<DocumentListTitleForm>(f => {
                f.Title = "General Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
             .AddForm<AddDocumentListItemsForm>(f =>
             {
                 f.Title = "Manage Documents";
                 f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
             })
            .AddView(v => {
                v.Id = "document-list";
                v.Title = "Document";
                v.Path = "~/UI/Views/Widgets/DocumentList.cshtml";
                //v.IconImage = "slideshow.svg";
                v.IconClass = "fa fa-file-text-o";
            }); ;
            
        }
    }
}
