using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.NavMenu.ViewModels;
using Angelo.Connect.NavMenu.Services;
using Angelo.Connect.NavMenu.Data;

namespace Angelo.Connect.NavMenu.UI.Controllers.Api
{
    public class NavMenuWidgetDataController : Controller
    {
        private NavMenuWidgetService _widgetService;

        public NavMenuWidgetDataController(NavMenuWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/navmenu")]
        public IActionResult UpdateWidget(NavMenuWidgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var widget = model.ProjectToModel();
                _widgetService.UpdateModel(widget);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

    }
}

