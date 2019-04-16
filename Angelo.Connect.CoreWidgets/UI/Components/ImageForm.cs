using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class ImageForm : ViewComponent
    {
        private ImageService _imageService;

        public ImageForm(ImageService imageService)
        {
            _imageService = imageService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Image model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
