using Angelo.Connect.Calendar.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.UI.Components
{
    public class CalendarWidgetSettings: ViewComponent
    {
        public CalendarWidgetSettings()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(CalendarWidgetSetting model)
        {
            return View(model);
        }
    }
}
