using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Services;
using Angelo.Connect.Video.Models;
using Microsoft.AspNetCore.Authorization;

namespace Angelo.Connect.Video.UI.Controllers.Api
{
    public class VideoBackgroundWidgetDataController : Controller
    {
        private VideoBackgroundWidgetService _videoService;

        public VideoBackgroundWidgetDataController(VideoBackgroundWidgetService videoService)
        {
            _videoService = videoService;
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/videobackground")]
        public IActionResult UpdateVideoLink(VideoBackgroundWidgetViewModel model)
        {
            if (ModelState.IsValid)
            {
                // If the full URL is entered, then extract just the video id.
                if (model.VideoSourceType == VideoSourceTypes.YouTube)
                {
                    model.VimeoVideoId = null;
                    model.YoutubeVideoId = ExtractYouTubeIdFromUrl(model.YoutubeVideoId);
                    model.SourceUri = "https://www.youtube.com/embed/" + model.YoutubeVideoId;
                }
                if (model.VideoSourceType == VideoSourceTypes.Vimeo)
                {
                    model.YoutubeVideoId = null;
                    model.VimeoVideoId = ExtractVimeoIdFromUrl(model.VimeoVideoId);
                    model.SourceUri = "https://player.vimeo.com/video/" + model.VimeoVideoId;
                }

                model.Autoplay = true;
                model.ShowPlayerControls = false;

                _videoService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete, Route("/api/widgets/videobackground")]
        public IActionResult DeleteVideoLink(VideoBackgroundWidgetViewModel model)
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

        private string ExtractVimeoIdFromUrl(string videoUrl)
        {
            var pos1 = videoUrl.LastIndexOf("/");
            if (pos1 > -1)
            {
                var idPart = videoUrl.Substring(pos1 + 1);
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

