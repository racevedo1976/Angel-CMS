using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Blog.Models;

namespace Angelo.Connect.Blog.UI.Components
{
    public class BlogWidgetForm : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(BlogWidget model)
        {
            model.Categories = null;
            model.Tags = null;

            return View("/UI/Views/Components/BlogWidgetForm.cshtml", model);
        }
    }
}
