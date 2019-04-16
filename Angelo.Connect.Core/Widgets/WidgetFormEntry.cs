using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;

namespace Angelo.Connect.Widgets
{
    public class WidgetFormEntry
    {
        public string Title { get; set; }
        public AjaxFlags AjaxFlags { get; set; }

        public List<string> Tabs { get; set; }

        internal WidgetConfig Widget { get; private set; }
        internal Type ComponentType { get; private set; }

        internal WidgetFormEntry(WidgetConfig widget, Type componentType)
        {
            Widget = widget;
            ComponentType = componentType;
            Tabs = new List<string>();
        }

        public HtmlString Render(ViewContext viewContext, string widgetId)
        {
            var services = viewContext.HttpContext.RequestServices;

            var settings = Widget.GetSettings(services, widgetId);
            var componentHelper = services.GetService<IViewComponentHelper>() as DefaultViewComponentHelper;

            componentHelper.Contextualize(viewContext);

            var content = componentHelper.InvokeAsync(ComponentType, new { model = settings }).Result;

            return content.ToHtmlString();
        }
       
    }
}
