using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AutoMapper.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Plugins;

namespace Angelo.Connect.Widgets
{
    public class WidgetProvider
    {
        private IServiceProvider _serviceProvider;
        private IRazorViewEngine _viewEngine;
        private IRazorPageActivator _pageActivator;
        private ITempDataProvider _tempDataProvider;
        private IModelMetadataProvider _modelMetadataProvider;
        private DefaultViewComponentHelper _componentHelper;
        private PluginContext _pluginContext;
        private IHttpContextAccessor _httpContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;
        private IEnumerable<IWidgetNamedModelProvider> _namedModelProviders;

        public IEnumerable<WidgetConfig> Widgets { get; set; }
        
        public WidgetProvider(
            IRazorViewEngine viewEngine,
            IRazorPageActivator pageActivator,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IModelMetadataProvider modelMetadataProvider,
            IHttpContextAccessor httpContextAccessor,
            IViewComponentHelper componentHelper,
            IEnumerable<IWidgetNamedModelProvider> namedModelProviders,
            IContextAccessor<SiteContext> siteContextAccessor,
            IContextAccessor<UserContext> userContextAccessor,
            PluginContext pluginContext
        )
        {
            _viewEngine = viewEngine;
            _pageActivator = pageActivator;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _modelMetadataProvider = modelMetadataProvider;
            _componentHelper = componentHelper as DefaultViewComponentHelper;
            _httpContextAccessor = httpContextAccessor;
            _siteContextAccessor = siteContextAccessor;
            _userContextAccessor = userContextAccessor;
            _pluginContext = pluginContext;

            _namedModelProviders = namedModelProviders;

            Widgets = pluginContext.GetData<WidgetConfig>();
        }

        public IEnumerable<string> GetCategories()
        {
            return Widgets.OrderBy(x => x.Category).Select(x => x.Category).Distinct().ToList();
        }

        public IEnumerable<WidgetConfig> GetByCategory(string category)
        {
            return Widgets.Where(x => x.Category == category);
        }

        public WidgetConfig GetWidgetConfig(string widgetType)
        {
            return Widgets.FirstOrDefault(x => x.WidgetType == widgetType);
        }

        public WidgetConfig GetWidgetConfig<TModel>(TModel model)
        {
            var config = Widgets.FirstOrDefault(x => x.ModelType == model.GetType());

            if (config == null)
                throw new NullReferenceException("Could not locate a configuration for " + model.GetType().FullName);

            return config;
        }

        public string GetView(string widgetType, string viewId)
        {
            var entry = GetWidgetConfig(widgetType);

            return entry.GetViewPath(viewId);
        }

        public string GetDefaultViewId(string widgetType)
        {
            var entry = GetWidgetConfig(widgetType);

            return entry.GetDefaultViewId();
        }

        public IEnumerable<IMenuItem> GetCustomMenuItems(string widgetType, string widgetId, string hostUrl)
        {
            var menuItems = new List<IMenuItem>();
            var widgetConfig = GetWidgetConfig(widgetType);

            var widgetContext = new WidgetContext
            {
                HttpContext = _httpContextAccessor.HttpContext,
                SiteContext = _siteContextAccessor.GetContext(),
                UserContext = _userContextAccessor.GetContext(),
                Settings = GetSettings(widgetType, widgetId),
                HostUrl = hostUrl,
            };

            foreach (var menuItemFunc in widgetConfig.MenuItems)
                menuItems.Add(menuItemFunc.Invoke(widgetContext));

            return menuItems;
        }

        public IWidgetModel GetSettings(string widgetType, string widgetId)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if (widget.ServiceType != null)
                return (IWidgetModel)widget.GetSettings(_serviceProvider, widgetId);
            
            return null;
        }

        public IWidgetModel GetDefaultSettings(string widgetType)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if(widget.ServiceType != null)
                return (IWidgetModel)widget.GetDefaultSettings(_serviceProvider);

            return null;
        }

        public IWidgetModel CloneSettings(string widgetType, string widgetId)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if (widget.ServiceType != null)
                return (IWidgetModel)widget.CloneSettings(_serviceProvider, widgetId);

            return null;
        }

        public IWidgetModel ExportSettings(string widgetType, string widgetId)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if (widget.ServiceType != null)
                return (IWidgetModel)widget.ExportSettings(_serviceProvider, widgetId);

            return null;
        }

        public IWidgetModel Create(string widgetType)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if (widget.ServiceType != null)
            {
                var model = GetDefaultSettings(widgetType);

                model.Id = Guid.NewGuid().ToString();
                widget.SaveSettings(_serviceProvider, model);

                return model;
            }

            return null;
        }

        public IWidgetModel Create(string widgetType, IWidgetModel model)
        {
            var widget = GetWidgetConfig(widgetType);

            if (widget == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            if (widget.ServiceType != null)
            {
                if (string.IsNullOrEmpty(model.Id))
                    model.Id = KeyGen.NewGuid();

                widget.SaveSettings(_serviceProvider, model);

                return model;
            }

            return null;
        }

        public IWidgetModel Create(string widgetType, string model)
        {
            var widgetConfig = GetWidgetConfig(widgetType);
            IWidgetModel widgetModel = null;

            if (widgetConfig == null)
                throw new NullReferenceException("Could not resolve widget service " + widgetType);

            model = model.Trim();

            if (!string.IsNullOrEmpty(model))
            {
                if (model.EndsWith(".json"))
                    widgetModel = GetModelFromJsonFile(widgetConfig, model);

                else if (model.StartsWith("{"))
                    widgetModel = GetModelFromJsonString(widgetConfig, model);

                else
                    widgetModel = GetNamedModel(widgetConfig, model);

                if(widgetModel != null)
                {
                    widgetModel.Id = KeyGen.NewGuid();
                    widgetConfig.SaveSettings(_serviceProvider, widgetModel);

                    return widgetModel;
                }
            }

            // otherwise just create using the default model supplied by the widget
            return Create(widgetType);
        }

        public TService GetService<TService>() where TService : IWidgetService
        {
            var widget = _serviceProvider.GetService<TService>();

            if(widget == null)
                throw new NullReferenceException("Could not resolve widget service " + typeof(TService).FullName);

            return widget;
        }

        public HtmlString Render(string widgetType, string viewId)
        {
            var widget = GetWidgetConfig(widgetType);
            var viewSettings = widget.Views.First(x => x.Id == viewId);
            var view = GetView(viewSettings.Path);
            var model = widget.GetDefaultSettings(_serviceProvider);

            return Compose(model, view);
        }

        public HtmlString Render(string widgetType, string viewId, string widgetId)
        {
            var widget = GetWidgetConfig(widgetType);
            var viewSettings = widget.Views.First(x => x.Id == viewId);
            var view = GetView(viewSettings.Path);
            var model = widget.GetSettings(_serviceProvider, widgetId);

            return Compose(model, view);
        }

        private HtmlString Compose(object viewModel, IView view)
        {
            using (var output = new StringWriter())
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var routeData = httpContext.GetRouteData() ?? new RouteData();
                var tempData = new TempDataDictionary(httpContext, _tempDataProvider);
                var viewData = new ViewDataDictionary<object>(_modelMetadataProvider, new ModelStateDictionary());
                var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());

                viewData.Model = viewModel;

                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    viewData,
                    tempData,
                    output,
                    new HtmlHelperOptions()
                );

                view.RenderAsync(viewContext).GetAwaiter().GetResult();

                return new HtmlString(output.ToString());
            }
        }

        private IWidgetModel GetNamedModel(WidgetConfig widget, string namedModel)
        {
            foreach (var modelProvider in _namedModelProviders)
            {
                var model = modelProvider.GetModel(widget.WidgetType, namedModel, widget.ModelType);

                if (model != null)
                {
                    return model;
                }
            }

            return null;
        }

        private IWidgetModel GetModelFromJsonString(WidgetConfig widget, string jsonModel)
        {
            if (jsonModel != null)
            {
                return JsonConvert.DeserializeObject(jsonModel, widget.ModelType) as IWidgetModel;
            }

            return null;
        }

        private IWidgetModel GetModelFromJsonFile(WidgetConfig widget, string jsonFilePath)
        {
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                var jsonModel = File.ReadAllText(jsonFilePath);

                if (string.IsNullOrEmpty(jsonModel.Trim()))
                    throw new NullReferenceException($"Cannot convert widget model. Null or empty file {jsonFilePath}");

                return GetModelFromJsonString(widget, jsonModel);
            }

            return null;
        }

        private IView GetView(string viewPath)
        {
            var viewEngineResult = _viewEngine.GetView(null, viewPath, false);

            if (!viewEngineResult.Success)
                throw new InvalidOperationException($"Could not find view template '{viewPath}'");

            return viewEngineResult.View;
        }
    }

}
