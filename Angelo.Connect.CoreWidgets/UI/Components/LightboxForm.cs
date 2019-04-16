using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class LightboxForm : ViewComponent
    {
        private LightboxService _lightboxService;

        public LightboxForm(LightboxService lightboxService)
        {
            _lightboxService = lightboxService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Lightbox model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
