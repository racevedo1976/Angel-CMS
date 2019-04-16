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
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class CorpSiteDetailsCreate : ViewComponent
    {
        private ClientManager _clientManager;

        public CorpSiteDetailsCreate(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        private async Task<List<SelectListItem>> GetClientSelectListAsync()
        {
            var clients = await _clientManager.GetAll();
            clients = clients.OrderBy(x => x.Name).ToList();
            var list = new List<SelectListItem>();
            foreach(var client in clients)
            {
                list.Add(new SelectListItem()
                {
                    Text = client.Name,
                    Value = client.Id
                });
            }
            return list;
        }

        public async Task<IViewComponentResult> InvokeAsync(string corpId = null)
        {
            if (string.IsNullOrEmpty(corpId))
                return new ViewComponentPlaceholder();

            var model = new SiteCreateViewModel();
            //model.Templates = await _clientManager.GetTemplatesAsync(model.ClientId);

            ViewData["ClientSelectList"] = await GetClientSelectListAsync();

            return View(model);
        }
    }
}

