using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.CoreWidgets.Services;
using Angelo.Connect.CoreWidgets.Models;

namespace Angelo.Connect.CoreWidgets.UI.Components
{
    public class TitleForm : ViewComponent
    {
        private TitleService _titleService;

        public TitleForm(TitleService titleService)
        {
            _titleService = titleService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Title model)
        {
            return await Task.Run(() =>  View(model));
        }
    }
}
