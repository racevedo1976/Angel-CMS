using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;

namespace Angelo.Connect.SlideShow.UI.Components
{
    public class SlideShowTypeForm : ViewComponent
    {
        private SlideShowService _slideShowService;

        public SlideShowTypeForm(SlideShowService slideShowService)
        {
            _slideShowService = slideShowService;
        }

        public async Task<IViewComponentResult> InvokeAsync(SlideShowWidget model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
