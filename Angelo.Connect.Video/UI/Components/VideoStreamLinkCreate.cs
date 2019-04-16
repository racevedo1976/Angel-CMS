using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Models;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoStreamLinkCreate : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string clientId = "")
        {
            if (string.IsNullOrEmpty(clientId))
                return new ViewComponentPlaceholder();
            else
            {
                var model = new VideoStreamLink()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "",
                    Path = "",
                    ClientId = clientId
                };
                return View(model);
            }
        }
    }
}
