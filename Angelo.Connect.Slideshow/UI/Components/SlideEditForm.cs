using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Slideshow.UI.Components
{
    public class SlideEditForm : ViewComponent
    {
        private SlideShowService _slideShowService;
        private IDocumentService<Slide> _slideService;
        private SlideShowWidgetService _widgetService;

        public SlideEditForm(SlideShowService slideShowService, IDocumentService<Slide> slideService,
            SlideShowWidgetService widgetService)
        {
            _slideShowService = slideShowService;
            _slideService = slideService;
            _widgetService = widgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, string slideId)
        {
            
            var slide = await GetSlideAsync(slideId);
            if (slide == null)
            {
                var defaultSettings = _widgetService.GetModel(widgetId);
                slide = new Slide()
                {
                    WidgetId = widgetId,
                    DocumentId = KeyGen.NewGuid(),
                    Color = defaultSettings.BackgroundColor,
                    Duration = defaultSettings.Duration,
                    ImageUrl = ""
                };
            }

            return View(slide);
        }

        public async Task<Slide> GetSlideAsync(string id)
        {
            return await _slideService.GetAsync(id);
        }
    }
}

