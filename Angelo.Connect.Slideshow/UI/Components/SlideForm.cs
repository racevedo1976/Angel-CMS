using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.SlideShow.UI.Components
{
    public class SlideForm : ViewComponent
    {
        private SlideShowService _slideShowService;
        private IDocumentService<Slide> _slideService;

        public SlideForm(SlideShowService slideShowService, IDocumentService<Slide> slideService)
        {
            _slideShowService = slideShowService;
            _slideService = slideService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Slide model)
        {
            var id = this.Request.Query["documentId"];
            if (model == null) model = string.IsNullOrEmpty(id)
                    ? CreateDefaultSlide()
                    : await GetSlideAsync(id);

            return await Task.Run(() => View(model));
        }

        private async Task<Slide> GetSlideAsync(string id)
        {
            return await _slideService.GetAsync(id);
        }

        private Slide CreateDefaultSlide()
        {
            return new Models.Slide()
            {
                DocumentId = KeyGen.NewGuid()
            };
        }
    }
}
