using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Rendering;
using Angelo.Connect.Services;

namespace Angelo.Connect.UI.Components
{
    // Just a simple wrapper that binds the page view to model used by some TagHelpers
    public class GenericView : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string view, object model = null)
        {
            return await Task.FromResult(
                View(view, model)
            );
        }
    }
}
