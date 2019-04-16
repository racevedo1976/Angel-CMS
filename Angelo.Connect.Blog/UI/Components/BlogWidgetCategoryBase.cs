using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.UI.ViewModels;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Blog.UI.Components
{
    public class BlogWidgetCategoryBase : ViewComponent
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogWidgetCategoryBase(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(BlogWidget model)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new BlogWidgetCategoryFormViewModel
            {
                WidgetId = model.Id,
                UserId = userContext.UserId
            };

            return View("/UI/Views/Components/BlogWidgetCategoryBase.cshtml", viewModel);
        }
    }
}
