using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Video.Services;
using Angelo.Connect.Video.Models;

namespace Angelo.Connect.Video.UI.Controllers.Api
{
    [Authorize]
    public class VideoWidgetDataController : Controller
    {
        private VideoWidgetService _videoService;

        public VideoWidgetDataController(VideoWidgetService videoService)
        {
            _videoService = videoService;
        }

        [HttpPost, Route("/api/widgets/video")]
        public IActionResult UpdateVideoLink(VideoWidgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                // If the full URL is entered, then extract just the video id.
                if (model.VideoSourceType == VideoSourceTypes.YouTube)
                    model.YouTubeVideoId = ExtractYouTubeIdFromUrl(model.YouTubeVideoId);
               
                _videoService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete, Route("/api/widgets/video")]
        public IActionResult DeleteVideoLink(VideoWidgetViewModel model)
        {
            if (model.Id != null)
            {
                _videoService.DeleteModel(model.Id);
                return Ok();
            }

            return BadRequest();
        }

        private string ExtractYouTubeIdFromUrl(string videoUrl)
        {
            var pos1 = videoUrl.IndexOf("?v=");
            if (pos1 == -1) pos1 = videoUrl.IndexOf("&v=");
            if (pos1 > -1)
            {
                var idPart = videoUrl.Substring(pos1 + 3);
                var pos2 = idPart.IndexOf("&");
                if (pos2 > -1)
                    return idPart.Substring(0, pos2);
                else
                    return idPart;
            }
            return videoUrl;
        }

    }
}

