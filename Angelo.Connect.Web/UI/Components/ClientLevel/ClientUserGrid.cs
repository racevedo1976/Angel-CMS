using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;


namespace Angelo.Connect.Web.UI.Components.ClientLevel
{
    public class ClientUserGrid : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string clientId, string directoryId)
        {

            ViewData["ClientId"] = clientId;
            ViewData["DirectoryId"] = directoryId;

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
