using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Widgets;
using Angelo.Connect.UI.Extensions;

namespace Angelo.Connect.Rendering.Components
{
    public class WidgetForm : ViewComponent
    {
        private WidgetProvider _widgetProvider;

        public WidgetForm(WidgetProvider widgetProvider)
        {
            _widgetProvider = widgetProvider;
        }

        public async Task<IViewComponentResult> InvokeAsync(string type, string id = null)
        {
            var widgetInfo = _widgetProvider.GetWidgetConfig(type);

            // If multiple forms are present, use the view that creates tab items
            // automatically (eg, the old method)
            var viewPath = "/UI/Views/Rendering/WidgetSettingsModal-V1.cshtml";

            // Otherwise use the new form
            // NOTE: All widgets will eventually use the new form 
            if (widgetInfo.Forms.Count() == 1)
            {
                viewPath = "/UI/Views/Rendering/WidgetSettingsModal-V2.cshtml";
            }

            // Sending id via ViewData to avoid creating a custom view model
            ViewData["WidgetId"] = id;

            // The underlying modal layout will add to modal's css if set
            ViewData["ModalCssClass"] = "widget-form-modal";

            return await Task.Run(() => View(viewPath, widgetInfo));
        }
    }
}
