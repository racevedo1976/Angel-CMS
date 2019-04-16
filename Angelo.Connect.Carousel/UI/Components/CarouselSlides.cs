using Angelo.Connect.Carousel.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Carousel.Services;

namespace Angelo.Connect.Carousel.UI.Components
{
    public class CarouselSlides : ViewComponent
    {
        private CarouselWidgetService _carouselWidgetService;

        public CarouselSlides(CarouselWidgetService carouselWidgetService)
        {
            _carouselWidgetService = carouselWidgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(CarouselWidget model)
        {
           // var model = _carouselWidgetService.GetModel(widgetId);
            
            return await Task.Run(() => View(model));
        }
    }
}
