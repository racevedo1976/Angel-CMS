using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.SlideShow.Services;
using Angelo.Connect.SlideShow.Models;

namespace Angelo.Connect.SlideShow.UI.Components
{
    public class GalleryItemSave : ViewComponent
    {
        private GalleryService _galleryService;

        public GalleryItemSave(GalleryService galleryService)
        {
            _galleryService = galleryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, string url)
        {
            var model = new GalleryItem();
            model.WidgetId = widgetId;
            model.Url = url;
            return await Task.Run(() =>  View(model));
        }
    }
}
