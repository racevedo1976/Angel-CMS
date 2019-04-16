using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;

namespace Angelo.Connect.SlideShow.UI.Components
{
    public class GalleryTitleForm : ViewComponent
    {
        private GalleryService _galleryService;

        public GalleryTitleForm(GalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(GalleryWidget model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
