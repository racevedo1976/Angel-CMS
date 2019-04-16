using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Announcement.Models;

namespace Angelo.Connect.Announcement.UI.Components
{
    public class AnnouncementWidgetForm : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(AnnouncementWidget model)
        {
            model.Categories = null;
            model.Tags = null;

            return View("/UI/Views/Components/AnnouncementWidgetForm.cshtml", model);
        }
    }
}
