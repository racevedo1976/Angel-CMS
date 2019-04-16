using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Slideshow.UI.Components
{
    [ViewComponent(Name = "SlideList")]
    public class SlideList : ViewComponent
    {
        private IDocumentService<Slide> _slideService;
        public SlideList(IDocumentService<Slide> slideService)
        {
            _slideService = slideService;
    }
        public async Task<IViewComponentResult> InvokeAsync(string widgetId)
        {
            var slides = await GetSlideListAsync(widgetId);

            return View(slides);

            //return View(slides);
        }

        private async Task<ICollection<Slide>> GetSlideListAsync(string id)
        {
            return _slideService.Query()
                .Where(x => x.WidgetId == id)
                .OrderBy(x => x.Position)
                .ToList();
        }
    }
}