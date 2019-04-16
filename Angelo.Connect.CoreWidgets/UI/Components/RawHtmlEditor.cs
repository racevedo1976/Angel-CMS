using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Widgets;
using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class RawHtmlEditor : ViewComponent, IWidgetComponent<RawHtml>
    {
        public async Task<IViewComponentResult> InvokeAsync(RawHtml model)
        {
            return await Task.Run(() => 
                View(model)
            );
        }
    }
}
