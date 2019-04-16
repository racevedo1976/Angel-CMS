using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Common.Extensions;
using Angelo.Connect.Widgets;

namespace Angelo.Plugins
{
    public static class PluginBuilderExtensions
    {

        /// <summary>
        /// Use this function to register Widgets that are not included in one of the Widget asseblies.
        /// <para>
        /// Note: You should only be using this function for debugging purposes.  Comment out any
        /// references to this method before creating the production build.
        /// </para>
        /// </summary>
        /*
        public void RegisterWidget<TWidget>() where TWidget : IPlugin, new()
        {
            if (!_pluginContext.Plugins.Where(p => p.GetType() == typeof(TWidget)).Any())
            {
                IPlugin widget = new TWidget();
                _pluginContext.Plugins.Concat(new[] { widget });
                widget.Startup(_pluginBuilder);
            }
        }
        */

        public static WidgetBuilder<TModel> RegisterWidget<TService, TModel>(this PluginBuilder pluginBuilder, Action<WidgetConfig> widgetBuilder)
           where TModel : class, IWidgetModel
           where TService : class, IWidgetService<TModel>
        {
            var widget = new WidgetConfig();


            //widget.WidgetType = typeof(TModel).FullName;
            widget.ServiceType = typeof(TService);
            widget.ModelType = typeof(TModel);

            widgetBuilder.Invoke(widget);

            if(widget.Id == null)
                widget.Id = widget.WidgetType;

            widget.GetDefaultSettings = (serviceProvider) => {
                var service = serviceProvider.GetService<TService>();

                return service.GetDefaultModel();
            };

            widget.GetSettings = (serviceProvider, id) => {
                var service = serviceProvider.GetService<TService>();

                return service.GetModel(id);
            };

            widget.SaveSettings = (serviceProvider, settings) => {
                var service = serviceProvider.GetService<TService>();

                service.SaveModel((TModel)settings);
            };

            widget.CloneSettings = (serviceProvider, id) => {
                var service = serviceProvider.GetService<TService>();
                var model = service.GetModel(id);

                return service.CloneModel(model);
            };

            widget.ExportSettings = (serviceProvider, id) => {
                var service = serviceProvider.GetService<TService>();
                var serviceType = service.GetType();
                TModel model = null;

                try
                {
                    // attempt to cast as an export service if the widget implements it
                    var exportService = service as IWidgetExportService<TModel>;
                    model = exportService.ExportModel(id);
                }
                catch(Exception)
                {
                    // otherwise get the default model since actual model might contain FKs
                    // that could result in existing site / customer data being rendered when the 
                    // widget is imported into a different site

                    try
                    {
                        model = service.GetDefaultModel();
                    }
                    catch(Exception)
                    {
                        model = null;
                    }
                }

                // ensure id is null for export
                if(model != null)
                    model.Id = null;

                return model;
            };

            pluginBuilder.PluginContext.AddData(widget);

            pluginBuilder.ConfigureServices(services => {
                services.AddTransient<TService>();
                services.AddTransient<IWidgetService, TService>();
                services.AddTransient<IWidgetService<TModel>, TService>();
            });

            return new WidgetBuilder<TModel>(widget);
        }

        public static WidgetBuilder<IWidgetModel> RegisterWidget(this PluginBuilder pluginBuilder, Action<WidgetConfig> widgetBuilder)
        {
            var widget = new WidgetConfig();

            widgetBuilder.Invoke(widget);

            widget.ServiceType = null;
            widget.ModelType = null;

            widget.GetDefaultSettings = (serviceProvider) => {
                return null;
            };

            widget.GetSettings = (serviceProvider, id) => {
                return null;
            };

            widget.SaveSettings = (serviceProvider, settings) => {
                /* no-op */
            };

            widget.CloneSettings = (serviceProvider, id) => {
                return new EmptyWidgetModel();
            };

            pluginBuilder.PluginContext.AddData(widget);

            return new WidgetBuilder<IWidgetModel>(widget);
        }

    }
}
