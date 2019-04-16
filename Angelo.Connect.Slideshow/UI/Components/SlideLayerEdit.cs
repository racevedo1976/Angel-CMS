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
    public class SlideLayerEdit : ViewComponent
    {
        private SlideShowService _slideService;
        public SlideLayerEdit(SlideShowService slideService)
        {
            _slideService = slideService;
        }
        public async Task<IViewComponentResult> InvokeAsync(string layerId, string slideId, string layerType)
        {
            var layerName = "Default";
            var layer = await GetSlideLayer(layerId);
            if (layer == null)
            {
                layer = new SlideLayer()
                {
                    Id = KeyGen.NewGuid(),
                    SlideId = slideId,
                    LayerType = layerType
                };
            }

            switch (layerType)
            {

                case "button":
                    layerName = "Button";
                    break;

                 //TODO image or video, not implemented. 
                //case 'image'

                //case 'video'

                default:
                    layerName = "Default";
                    break;
            }

            return View(layerName, layer);
        }

        private async Task<SlideLayer> GetSlideLayer(string id)
        {
            return await _slideService.GetLayerAsync(id);
        }
    }
}