using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.News.Models;

namespace Angelo.Connect.News.UI.Components
{
    public class NewsWidgetForm : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(NewsWidget model)
        {
            model.Categories = null;
            model.Tags = null;

            return View("/UI/Views/Components/NewsWidgetForm.cshtml", model);
        }
    }
}
