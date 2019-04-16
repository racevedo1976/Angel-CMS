using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class SlideShowForm : ViewComponent
    {
        private SlideShowService _slideShowService;

        public SlideShowForm(SlideShowService slideShowService)
        {
            _slideShowService = slideShowService;
        }

        public async Task<IViewComponentResult> InvokeAsync(SlideShow model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
