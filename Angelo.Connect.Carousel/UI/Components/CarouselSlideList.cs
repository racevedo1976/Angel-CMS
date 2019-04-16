using Angelo.Connect.Carousel.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Carousel.UI.Components
{
    public class CarouselSlideList : ViewComponent
    {
        private CarouselWidgetService _carouselWidgetService;

        public CarouselSlideList(CarouselWidgetService carouselWidgetService)
        {
            _carouselWidgetService = carouselWidgetService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string widgetId)
        {
            var model = _carouselWidgetService.GetModel(widgetId);

            return await Task.Run(() => View(model));
        }
    }
}