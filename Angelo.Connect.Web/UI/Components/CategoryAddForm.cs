using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.Components
{
    public class CategoryAddForm : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string clientId, string siteId, string level, string ownerId)
        {
            ViewData["ClientId"] = clientId;
            ViewData["SiteId"] = siteId;

            var model = new CategoryViewModel();

            if (level == "Global")
            {
                model.OwnerLevel = OwnerLevel.Global;
            }
            else if (level == "Client")
            {
                model.OwnerLevel = OwnerLevel.Client;
            }
            else if (level == "Site")
            {
                model.OwnerLevel = OwnerLevel.Site;
            }
            else
            {
                model.OwnerLevel = OwnerLevel.User;
            }

            model.OwnerId = ownerId;

            return View(model);
        }
    }
}
