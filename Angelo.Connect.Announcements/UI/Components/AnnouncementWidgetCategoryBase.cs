using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.UI.ViewModels;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Announcement.UI.Components
{
    public class AnnouncementWidgetCategoryBase : ViewComponent
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementWidgetCategoryBase(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(AnnouncementWidget model)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new AnnouncementWidgetCategoryFormViewModel
            {
                WidgetId = model.Id,
                UserId = userContext.UserId
            };

            return View("/UI/Views/Components/AnnouncementWidgetCategoryBase.cshtml", viewModel);
        }
    }
}
