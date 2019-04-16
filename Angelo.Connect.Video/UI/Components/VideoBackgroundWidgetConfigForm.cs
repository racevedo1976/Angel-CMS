using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Video.Models;
using Kendo.Mvc.UI;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Video.Services;
using Angelo.Connect.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Security;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoBackgroundWidgetConfigForm : ViewComponent
    {
        ConnectDbContext _db;

        public VideoBackgroundWidgetConfigForm(ConnectDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync(VideoBackgroundWidgetViewModel model)
        {
            var positionList = new List<SelectListItem>();
            positionList.Add(new SelectListItem()
            {
                Value = "fixed",
                Text = "Fixed",
                Selected = false
            });
            positionList.Add(new SelectListItem()
            {
                Value = "static",
                Text = "Static",
                Selected = true
            });
            ViewBag.PositioningOptions = positionList;

            var sourceList = new List<SelectListItem>();
            sourceList.Add(new SelectListItem()
            {
                Value = "vimeo",
                Text = "Vimeo",
                Selected = true
            });
            ViewBag.VideoSourceTypes = sourceList;

            sourceList.Add(new SelectListItem()
            {
                Value = "youtube",
                Text = "Youtube",
                Selected = false
            });
            

            return await Task.Run(() => View(model));
        }
    }
}
