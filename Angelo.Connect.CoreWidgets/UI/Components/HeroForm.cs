using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class HeroForm : ViewComponent
    {
        private HeroService _widgetService;

        public HeroForm(HeroService widgetService)
        {
            _widgetService = widgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(HeroUnit model)
        {
            return await Task.Run(() => View(model));
        }
    }
}
