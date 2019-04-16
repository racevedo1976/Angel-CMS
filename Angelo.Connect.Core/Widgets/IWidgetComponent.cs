using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Angelo.Connect.Widgets
{
    public interface IWidgetComponent<TWidgetModel> where TWidgetModel : class, IWidgetModel
    {
        Task<IViewComponentResult> InvokeAsync(TWidgetModel settings);
    }
}
