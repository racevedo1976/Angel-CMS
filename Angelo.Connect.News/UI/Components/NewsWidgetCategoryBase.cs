using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.UI.ViewModels;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.News.UI.Components
{
    public class NewsWidgetCategoryBase : ViewComponent
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsWidgetCategoryBase(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(NewsWidget model)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new NewsWidgetCategoryFormViewModel
            {
                WidgetId = model.Id,
                UserId = userContext.UserId
            };

            return View("/UI/Views/Components/NewsWidgetCategoryBase.cshtml", viewModel);
        }
    }
}
