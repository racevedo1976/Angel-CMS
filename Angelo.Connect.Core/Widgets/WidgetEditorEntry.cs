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
    public class WidgetEditorEntry
    {
        public WidgetConfig Widget { get; private set; }
        public Type ComponentType { get; private set; }

        internal WidgetEditorEntry(WidgetConfig widget, Type componentType)
        {
            Widget = widget;
            ComponentType = componentType;
        }

        public HtmlString Render(ViewContext viewContext, object model)
        {
            var services = viewContext.HttpContext.RequestServices;
            var componentHelper = services.GetService<IViewComponentHelper>() as DefaultViewComponentHelper;

            componentHelper.Contextualize(viewContext);

            var content = componentHelper.InvokeAsync(ComponentType, model).Result;

            return content.ToHtmlString();
        }
    }
}
