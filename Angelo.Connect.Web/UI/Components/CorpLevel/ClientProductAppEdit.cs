using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class ClientProductAppEdit : ViewComponent
    {
        private ClientManager _clientManager;
        private ProductManager _productManager;

        public ClientProductAppEdit(ClientManager clientManager, ProductManager productManager)
        {
            _clientManager = clientManager;
            _productManager = productManager;
        }

        protected async Task<List<SelectListItem>> GetProductSelectList()
        {
            var products = await _productManager.GetActiveProductsAsync();
            var sortedProducts = products.OrderBy(x => x.Name);
            var productList = new List<SelectListItem>();
            foreach (var product in sortedProducts)
            {
                productList.Add(new SelectListItem()
                {
                    Text = product.Name,
                    Value = product.Id
                });
            }
            return productList;
        }

        protected async Task<List<SelectListItem>> GetAddonSelectList(string productId)
        {
            var addons = new List<SelectListItem>();
            var allAddons = await _productManager.GetAddOnsOfProductId(productId);
            var sortedAddons = allAddons.OrderBy(x => x.Name);
            foreach (var addon in sortedAddons)
            {
                addons.Add(new SelectListItem()
                {
                    Value = addon.Id,
                    Text = addon.Name
                });
            }
            return addons;
        }

        public async Task<IViewComponentResult> InvokeAsync(string appId = null, string clientId = null)
        {
            ClientProductAppViewModel model;
            if (appId == null)
            {
                // Add Product
                model = new ClientProductAppViewModel();
                model.ClientId = clientId;
                model.SubscriptionStartUTC = DateTime.UtcNow;
            }
            else
            {
                // Edit Product
                var app = await _clientManager.GetProductAsync(appId);
                model = app.ProjectTo<ClientProductAppViewModel>();
                model.ClientProductAppId = app.Id;
            }
            var appAddons = await _clientManager.GetProductAddOnsAsync(model.ClientProductAppId);
            model.AddonIds = appAddons.Select(x => x.Id).ToList();

            ViewBag.AvailableProducts = await GetProductSelectList();
            ViewBag.AvailableAddons = await GetAddonSelectList(model.ProductId);

            return View(model);
        }
    }
}

