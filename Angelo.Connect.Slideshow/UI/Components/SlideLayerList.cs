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
    public class SlideLayerList : ViewComponent
    {
        private IDocumentService<Slide> _slideService;
        public SlideLayerList(IDocumentService<Slide> slideService)
        {
            _slideService = slideService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string slideId)
        {
            var layers = new List<SlideLayer>();

            var slide = GetSlideAsync(slideId);
            if (slide.Layers != null)
                layers = slide.Layers.ToList();

            return View(layers);
        }

        private Slide GetSlideAsync(string id)
        {
            return _slideService.Query().FirstOrDefault(x => x.DocumentId == id);
        }
    }
}