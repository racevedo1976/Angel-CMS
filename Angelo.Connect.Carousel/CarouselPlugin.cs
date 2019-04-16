using Angelo.Common.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.Icons;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Plugins;
using System;
using Angelo.Connect.Carousel.Models;
using Angelo.Connect.Carousel.Services;
using Angelo.Connect.Carousel.Data.Migrations;
using Angelo.Connect.Carousel.Data;
using Angelo.Connect.Carousel.UI.Components;

namespace Angelo.Connect.Carousel
{
    public class CarouselPlugin : IPlugin
    {
        public CarouselPlugin()
        {
        }

        public string Author { get; } = "SchoolInSites";
        public string Description { get; } = "Carousel Plugin";
        public string Name { get; } = "Carousel Plugin";
        public string Version { get; } = "0.0.1";
        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services =>
            {
               
                services.AddTransient<CarouselSlideService>();

                // register migrations
                services.AddTransient<IAppMigration, P810000_CreateInitialDocumentTables>();
                
            });

            pluginBuilder.AddDbContext<CarouselDbContext>();

            pluginBuilder.RegisterWidget<CarouselWidgetService, CarouselWidget>(widget =>
                {
                    widget.WidgetType = "carousel";
                    widget.WidgetName = "Carousel";
                    widget.Category = "Text";
                })

               //these addForms below must have a parameter named "model"
                .AddForm<CarouselSlides>(f =>
                {
                    f.Title = "Manage Carousel Slides";
                    f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
                })
                .AddView(v => {
                    v.Id = "carousel";
                    v.Title = "Carousel";
                    v.Path = "~/UI/Views/Widgets/Carousel.cshtml";
                    v.IconClass = "fa fa-file-text-o";
                }); ;

        }
    }
}
