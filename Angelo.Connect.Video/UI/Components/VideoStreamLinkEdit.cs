using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Models;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Video.Services;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoStreamLinkEdit : ViewComponent
    {
        private VideoStreamLinkService _linkService;

        public VideoStreamLinkEdit(VideoStreamLinkService linkService)
        {
            _linkService = linkService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            var model = await _linkService.GetVideoStreamLinkAsync(id);
            if (model == null)
                return new ViewComponentPlaceholder();
            else
                return View(model);
        }
    }
}
