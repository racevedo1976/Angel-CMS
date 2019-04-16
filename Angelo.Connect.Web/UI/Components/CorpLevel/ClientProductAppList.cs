using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Kendo.Mvc.UI;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientProductAppList : ViewComponent
    {
        private ClientManager _clientManager;

        public ClientProductAppList(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        private string MegabytesToText(int megabytes)
        {
            if (megabytes < 1000)
                return megabytes.ToString("#,##0") + " MB";
            else
                return String.Format("{0:#,##0.##}", (megabytes / 1000.0)) + " GB";
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = null)
        {
            var model = new List<ClientProductAppListItemViewModel>();
            var apps = await _clientManager.GetProductsAsync(clientId);
            foreach (var item in apps)
            {
                var context = await _clientManager.GetProductContextAsync(item.Id);
                model.Add(new ClientProductAppListItemViewModel()
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    ClientProductAppId = item.Id,
                    ClientId = clientId,
                    ProductId = item.Product.Id,
                    AddOnId = "",
                    SiteCount = context.ActiveSiteCount.ToString() + "/" + item.MaxSiteCount.ToString(),
                    TotalSpace = MegabytesToText(context.TotalMB)
                });
            }
            return View(model);
        }
    }
}
