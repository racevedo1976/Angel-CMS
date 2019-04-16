using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.ViewModels;

namespace Angelo.Connect.Assignments.UI.Controllers.Api
{
    public class VideoWidgetDataController : Controller
    {
        private AssignmentsWidgetService _widgetService;

        public VideoWidgetDataController(AssignmentsWidgetService widgetService)
        {
            _widgetService = widgetService;
        }

        [HttpPost, Route("/api/widgets/assignments")]
        public IActionResult UpdateVideoLink(AssignmentsWidgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                _widgetService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete, Route("/api/widgets/assignments")]
        public IActionResult DeleteVideoLink(AssignmentsWidgetViewModel model)
        {
            if (model.Id != null)
            {
                _widgetService.DeleteModel(model.Id);
                return Ok();
            }

            return BadRequest();
        }

    }
}

