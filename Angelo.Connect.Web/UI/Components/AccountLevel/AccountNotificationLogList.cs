using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Security;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.Components
{
    public class AccountNotificationLogList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string notificationId, string logType)
        {
            if (string.IsNullOrEmpty(notificationId))
                return new ViewComponentPlaceholder();

            ViewData["notificationId"] = notificationId;
            ViewData["logType"] = logType ?? string.Empty;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
