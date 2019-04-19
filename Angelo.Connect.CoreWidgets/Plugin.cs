using System;
using System.Collections.Generic;

using Angelo.Connect.CoreWidgets.Data;
using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.UI.Components;
using Angelo.Connect.Widgets;
using Angelo.Connect.Widgets.Models;
using Angelo.Connect.Widgets.Services;
using Angelo.Connect.Menus;
using Angelo.Connect.Icons;
using Angelo.Plugins;


namespace Angelo.Connect.CoreWidgets
{
    public class Plugin : IPlugin
    {
        public string Name { get; } = "Core Widgets Plugin";
        public string Description { get; } = "Plugin containing multiple widgets for use with the cms framework.";
        public string Version { get; } = "0.0.1-beta1";
        public string Author { get; } = "MySites";

        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.AddDbContext<HtmlDbContext>(dbContext => {
                HtmlDbActions.CreateSchemas(dbContext);
                HtmlDbActions.CreateTables(dbContext);
            });

            //-- HTML Widget--------
            pluginBuilder.RegisterWidget<RawHtmlService, Models.RawHtml>(widget => 
            {
                widget.WidgetType = "html";
                widget.WidgetName = "Rich Text";
                widget.Category = "Text";
            })
            .AddEditor<RawHtmlEditor>()
            .AddForm<RawHtmlForm>(f => {
                f.Title = "General Settings";
            })
            .AddView(v => {
                v.Id = "html-raw";
                v.Title = "HTML Editor";
                v.Path = "~/UI/Views/Widgets/RawHtml.cshtml";
                v.IconClass = "fa fa-code";
            });

            //-- Icon Widget--------
            pluginBuilder.RegisterWidget<IconService, Models.Icon>(widget => {
                widget.WidgetType = "icon";
                widget.WidgetName = "Icon";
                widget.Category = "Text";
            })
            .AddForm<IconForm>(f => {
                f.Title = "Icon Settings";
                f.Tabs =  new List<string>() { "Browse Icons", "Advanced Settings", "Styling" };
            })
            .AddView(v => {
                v.Id = "icon-1";
                v.Title = "Icon";
                v.Path = "~/UI/Views/Widgets/Icon1.cshtml";
                v.IconClass = "fa fa-flag";
            });

            //-- Image Widget--------
            pluginBuilder.RegisterWidget<ImageService, Models.Image>(widget => {
                widget.WidgetType = "image";
                widget.WidgetName = "Image Content";
                widget.Category = "Media";
                widget.IconUrl = "/assets/images/widgets/images-icon.png";
            })
            .AddForm<ImageForm>(f => {
                f.Title = "Default Settings";
            })
            .AddView(v => {
                v.Id = "image-inline";
                v.Title = "Image";
                v.Path = "~/UI/Views/Widgets/Image1.cshtml";
                v.IconClass = "fa fa-picture-o";
            });

            //-- Title Widget--------
            pluginBuilder.RegisterWidget<TitleService, Models.Title>(widget =>
            {
                widget.WidgetType = "title";
                widget.WidgetName = "Titles";
                widget.Category = "Text";
            })
            .AddEditor<TitleEditor>()
            .AddForm<TitleForm>(f => {
                f.Title = "Default Settings";
            })
            .AddView(v => {
                v.Id = "title-default";
                v.Title = "Title";
                v.Path = "~/UI/Views/Widgets/Title.cshtml";
                v.IconClass = "fa fa-text-height";
            });

             //-- Alert Widget--------
            pluginBuilder.RegisterWidget<AlertService, Models.Alert>(widget => {
                widget.WidgetType = "alert";
                widget.WidgetName = "Page Alerts";
                widget.Category = "Text";
            })
            .AddEditor<AlertEditor>()
            .AddForm<AlertForm>(f => {
                f.Title = "Default Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
            .AddView(v => {
                v.Id = "alert-alert1";
                v.Title = "Page Alert";
                v.Path = "~/UI/Views/Widgets/Alert1.cshtml";
                v.IconClass = "fa fa-bell";
            });


            //-- Hero Widget--------
            pluginBuilder.RegisterWidget<HeroService, Models.HeroUnit>(widget => {
                widget.WidgetType = "hero";
                widget.WidgetName = "Hero Unit";
                widget.Category = "Text";
                widget.IconUrl = "/assets/images/widgets/hero-icon.png";
            })
            .AddEditor<HeroEditor>()
            .AddForm<HeroForm>(f => {
                f.Title = "Default Settings";
            })
            .AddView(v => {
                v.Id = "hero-hero1";
                v.Title = "Hero Unit";
                v.Path = "~/UI/Views/Widgets/Hero1.cshtml";
                v.IconClass = "fa fa-bell";
            });

            //-- Search Widget--------
            pluginBuilder.RegisterWidget<SearchService, Models.Search>(widget => {
                widget.WidgetType = "search";
                widget.WidgetName = "Search";
                widget.Category = "Text";
            })
            .AddView(v => {
                v.Id = "search-search1";
                v.Title = "Search";
                v.Path = "~/UI/Views/Widgets/Search.cshtml";
                v.IconClass = "fa fa-search";
            });

            //-- Google Translate Widget--------
            pluginBuilder.RegisterWidget<GoogleTranslateService, Models.GoogleTranslate>(widget => {
                    widget.WidgetType = "translate";
                    widget.WidgetName = "Translate";
                    widget.Category = "Text";
                })
                .AddView(v => {
                    v.Id = "search-search1";
                    v.Title = "Translate";
                    v.Path = "~/UI/Views/Widgets/GoogleTranslate.cshtml";
                    v.IconClass = "fa fa-google";
                });


            //-- Lightbox Widget--------
            pluginBuilder.RegisterWidget<LightboxService, Models.Lightbox>(widget => {
                widget.WidgetType = "lightbox";
                widget.WidgetName = "Lightbox";
                widget.Category = "Advanced";
            })
            .AddForm<LightboxForm>(f => {
                f.Title = "Default Settings";
            })
            .AddMenuOption((context) => {
                return new MenuItemSecure
                {
                    Icon = IconType.Open,
                    Title = "Open Lightbox",
                    Url = $"javascript: void $.trigger('lightbox.open', '{context.Settings.Id}')",
                };
            })
            .AddView(v => {
                v.Id = "lightbox-1";
                v.Title = "Lightbox";
                v.Path = "~/UI/Views/Widgets/Lightbox1.cshtml";
                v.IconClass = "fa fa-external-link";
            });

            //-- ContactForm Widget--------
            pluginBuilder.RegisterWidget<ContactFormService, Models.ContactForm>(widget => {
                widget.WidgetType = "contactform";
                widget.WidgetName = "Contact Form";
                widget.Category = "Advanced";
            })
            .AddForm<ContactFormSettings>(f => {
                f.Title = "Default Settings";
            })
            .AddView(v => {
                v.Id = "contactform-1";
                v.Title = "Contact Form";
                v.Path = "~/UI/Views/Widgets/ContactForm1.cshtml";
                v.IconClass = "fa fa-envelope";
            });


            //
            // Deprecated / Old Sample Widgets
            //

            #region Old Sample widgets

            //-- Slideshow -----
            /*
            pluginBuilder.RegisterWidget<SlideShowService, Models.SlideShow>(widget => {
                widget.WidgetType = "slideshow";
                widget.WidgetName = "Image Gallery";
                widget.Category = "Media";
                widget.ShowInToolbar = false;
            })
            .AddForm<SlideShowForm>(f => {
                f.Title = "Default Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
            .AddView(v => {
                v.Id = "slideshow-carousel";
                v.Title = "Slideshow";
                v.Path = "~/UI/Views/Widgets/SlideShow1.cshtml";
                v.IconClass = "fa fa-camera";
            });

            //-- NavBarWidget-------------
            pluginBuilder.RegisterWidget<NavBarService, Models.NavBar>(widget => {
                widget.WidgetType = "navbar";
                widget.WidgetName = "Navigation Bar";
                widget.Category = "Menus";
                widget.ShowInToolbar = false;
            })
            .AddForm<NavBarForm>(f => {
                f.Title = "Default Settings";
            })
            .AddForm<NavBarMenuForm>(f => {
                f.Title = "Data Source";
            })
            .AddView(v => {
                v.Id = "navbar-top";
                v.Title = "Top Navigation";
                v.Path = "~/UI/Views/Widgets/TopNavBar.cshtml";
                v.IconClass = "fa fa-bars";
            });
            */



            #endregion
        }
    }
}