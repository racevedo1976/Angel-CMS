using Angelo.Connect.Carousel.Models;
using Angelo.Connect.Carousel.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Carousel.UI.Components
{
    public class EditCarouselSlide : ViewComponent
    {
        private CarouselWidgetService _carouselWidgetService;

        public EditCarouselSlide(CarouselWidgetService carouselWidgetService)
        {
            _carouselWidgetService = carouselWidgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId, string slideId)
        {
            var model = new CarouselSlide();

            if (string.IsNullOrEmpty(slideId))
            {
                model.Id = Guid.NewGuid().ToString("N");
                model.WidgetId = widgetId;
                model.LinkTarget = "_self";
            }
            else
            {
                model =  _carouselWidgetService.GetModel(widgetId).Slides?.FirstOrDefault(x => x.Id == slideId);

            }
           
            return await Task.Run(() => View(model));
        }
    }
}
