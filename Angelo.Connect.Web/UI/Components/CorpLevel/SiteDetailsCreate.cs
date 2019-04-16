using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Html;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteDetailsCreate : ViewComponent
    {
        private ClientManager _clientManager;

        public SiteDetailsCreate(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = "", string appId = "")
        {
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(appId))
                return new ViewComponentPlaceholder();

            var model = new SiteCreateViewModel();
            model.ClientId = clientId;
            model.ClientProductAppId = appId;
            model.Templates = await _clientManager.GetTemplatesAsync(model.ClientId);
            return View("SiteDetailsCreate", model);
        }
    }
}

