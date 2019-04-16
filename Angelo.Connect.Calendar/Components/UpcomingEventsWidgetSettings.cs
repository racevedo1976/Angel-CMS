using Angelo.Connect.Calendar.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.UI.Components
{
    public class UpcomingEventsWidgetSettings: ViewComponent
    {
        public UpcomingEventsWidgetSettings()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(UpcomingEventsWidget model)
        {
            return View(model);
        }
    }
}
