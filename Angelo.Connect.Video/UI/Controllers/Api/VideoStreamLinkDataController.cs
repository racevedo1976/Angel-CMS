using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Video.Services;
using Angelo.Connect.Video.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace Angelo.Connect.Video.UI.Controllers.Api
{
    [Authorize]
    public class VideoStreamLinkDataController : Controller
    {
        private VideoStreamLinkService _linkService;

        public VideoStreamLinkDataController(VideoStreamLinkService linkService)
        {
            _linkService = linkService;
        }

        [HttpPost, Route("/api/clients/videostreamlinks/data")]
        public async Task<JsonResult> GetClientVideoStreamLinks([DataSourceRequest] DataSourceRequest request, string clientId)
        {
            var model = await _linkService.GetClientVideoStreamLinksAsync(clientId);
            var result = model.AsQueryable().ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost, Route("/api/clients/videostreamlink")]
        public async Task<ActionResult> InsertVideoStreamLink(VideoStreamLink model)
        {
            if (ModelState.IsValid)
            {
                await _linkService.InsertVideoStreamLink(model);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [HttpPut, Route("/api/clients/videostreamlink")]
        public async Task<ActionResult> UpdateVideoStreamLink(VideoStreamLink model)
        {
            if (ModelState.IsValid)
            {
                await _linkService.UpdateVideoStreamLink(model);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete, Route("/api/clients/videostreamlink")]
        public async Task<ActionResult> DeleteVideoStreamLink(VideoStreamLink model)
        {
            await _linkService.DeleteVideoStreamLink(model.Id);
            return Ok(model);
        }

    }
}
