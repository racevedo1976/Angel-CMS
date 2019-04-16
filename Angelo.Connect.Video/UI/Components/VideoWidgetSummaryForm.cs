using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Models;
using Newtonsoft.Json;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoWidgetSummaryForm : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(VideoWidgetViewModel model)
        {
            var modelText = JsonConvert.SerializeObject(model, new JsonSerializerSettings { Formatting = Formatting.Indented });
            ViewData["modelText"] = modelText;

            return View(model);
        }
    }
}
