using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;


using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.SlideShow.Data;
using Angelo.Connect.Icons;
using Angelo.Connect.SlideShow.UI.Components;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Plugins;
using Angelo.Common.Migrations;
using Angelo.Connect.Slideshow.Data.Migrations;

namespace Angelo.Connect.SlideShow
{
    public class SlideShowPlugin : IPlugin
    {
        public SlideShowPlugin()
        {
        }

        public string Author { get; } = "MySites";
        public string Description { get; } = "Initial Slideshow Plugin";
        public string Name { get; } = "Slideshow Plugin";
        public string Version { get; } = "0.0.1";
        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services =>
            {
                var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();

                // register migrations
                services.AddTransient<IAppMigration, P610000_CreateInitialSlideShowTables>();
                services.AddTransient<IAppMigration, P610001_AddStylingFieldsToLayers>();
                services.AddTransient<IAppMigration, P610002_AddVideoBgToSlides>();
                services.AddTransient<IAppMigration, P610003_IncreaseTitleAndUrlColumnSize>();

                services.AddTransient<SlideShowService>();
                services.AddTransient<GalleryService>();
                services.AddTransient<IFolderManager<Slide>, FolderManager<Models.Slide>>();
                services.AddTransient<IDocumentService<Slide>, SlideDocumentService>();
            });

            pluginBuilder.AddDbContext<SlideShowDbContext>(db =>
            {
                SlideShowDbSeed.CreateSchemas(db);
                SlideShowDbSeed.CreateTables(db);
                SlideShowDbSeed.InsertSeedData(db);
            });

            pluginBuilder.AddDbContext<GalleryDbContext>(db =>
            {
                GalleryDbSeed.CreateTables(db);
            });

            pluginBuilder.AddStartupAction<SlideShowDocSeed>();

            pluginBuilder.RegisterWidget<SlideShowWidgetService, SlideShowWidget>(widget =>
            {
                widget.WidgetType = "slideshow lux";
                widget.WidgetName = "Slideshow";
                widget.Category = "Media";
            })
            .AddForm<SlideShowTitleForm>(f => {
                f.Title = "General Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
            //.AddForm<SlideShowTypeForm>(f =>
            //{
            //    f.Title = "Slideshow Type / Layout";
            //    f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            //})
            //.AddForm<SlideShowDefaultsForm>(f =>
            //{
            //    f.Title = "General Defaults";
            //    f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            //})
            .AddForm<AddSlidesForm>(f =>
            {
                f.Title = "Add / Manage Slide(s)";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
            .AddView(v => {
                v.Id = "slideshow-carousel";
                v.Title = "Slideshow";
                v.Path = "~/UI/Views/Widgets/SlideShowLux1.cshtml";
                //v.IconImage = "slideshow.svg";
                v.IconClass = IconType.Slideshow.ToString();
            });;

            pluginBuilder.RegisterWidget<GalleryWidgetService, GalleryWidget>(widget =>
            {
                widget.WidgetType = "gallery";
                widget.WidgetName = "Media Gallery";
                widget.Category = "Media";
            })
            .AddForm<GalleryTitleForm>(f => {
                f.Title = "General Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
             .AddForm<AddGalleryItemsForm>(f =>
             {
                 f.Title = "Manage Gallery items";
                 f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
             })
            .AddView(v => {
                v.Id = "media-gallery";
                v.Title = "Image Gallery";
                v.Path = "~/UI/Views/Widgets/Gallery.cshtml";
            //v.IconImage = "slideshow.svg";
                v.IconClass = IconType.Slideshow.ToString();
            });;
        }
    }
}
