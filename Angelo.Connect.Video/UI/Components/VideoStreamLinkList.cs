using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Models;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoStreamLinkList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            ViewData["clientId"] = clientId;
            var model = new List<VideoStreamLink>(); // send empty model so Telerik grid will work with intellisence.
            return View(model);
        }
    }
}
