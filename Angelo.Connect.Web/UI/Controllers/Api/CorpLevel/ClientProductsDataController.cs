using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using AutoMapper.Extensions;

using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class ClientProductAppsDataController : BaseController
    {
        private ClientManager _clientManager;
        private ProductManager _productManager;

        public ClientProductAppsDataController
        (
            ClientManager clientManager, 
            ProductManager productManager, 
            ILogger<ClientProductAppsDataController> logger
        ) : base(logger)
        {
            _clientManager = clientManager;
            _productManager = productManager;
        }

        [Authorize(policy: PolicyNames.CorpClientsRead)]
        [HttpPost, Route("/sys/corp/api/client/app/datasource")]
        public async Task<JsonResult> GetClientAppsDataSourceResult([DataSourceRequest] DataSourceRequest request, string clientId)
        {
            var list = new List<SelectListItem>();
            var apps = await _clientManager.GetProductsAsync(clientId);
            foreach (var app in apps)
            {
                list.Add(new SelectListItem()
                {
                    Text = app.Title,
                    Value = app.Id
                });
            }
            var result = list.ToDataSourceResult(new DataSourceRequest());
            return Json(result);
        }

        [HttpPost, Route("/api/client/template/datasource")]
        public async Task<JsonResult> GetClientTemplateDataSourceResult([DataSourceRequest] DataSourceRequest request, string clientId)
        {
            var list = new List<SelectListItem>();
            var templates = await _clientManager.GetTemplatesAsync(clientId);
            var result = templates.OrderBy(x => x.Title).ToDataSourceResult(new DataSourceRequest());
            return Json(result);
        }

        [Authorize(policy: PolicyNames.CorpProductsMap)]
        [HttpPost, Route("/sys/corp/api/client/app")]
        public async Task<ActionResult> SaveClientProductApp(ClientProductAppViewModel model)
        {
            if (ModelState.IsValid)
            {
                var app = model.ProjectTo<ClientProductApp>();
                app.Id = model.ClientProductAppId;
                app.Client = null;
                app.Product = null;
                await _clientManager.AddProductAsync(app, model.AddonIds);
                model.ClientProductAppId = app.Id;
                return Ok(model);
            }
            return BadRequest(model);
        }

        [Authorize(policy: PolicyNames.CorpClientsEdit)]
        [HttpDelete, Route("/sys/corp/api/client/app/{appId}")]
        public async Task<ActionResult> DeleteClientProductApp(string appId)
        {
            if (string.IsNullOrEmpty(appId) == false)
            {
                try
                {
                    await _clientManager.RemoveProductAsync(appId);
                    return Ok(ModelState);
                }
                catch (Exception e)
                {
                    // To Do: Log Error
                    //var s = e.Message;
                    //var t = e.InnerException.Message;
                    //ErrorView(e.Message);

                    return BadRequest(e.Message);
                }
            }

            return BadRequest();
        }

        [Authorize(policy: PolicyNames.CorpProductsMap)]
        [HttpPost, Route("/sys/corp/api/client/apps/addons")]
        public async Task<JsonResult> GetProductAddonsAsSelectList(string productId)
        {
            var Items = new List<SelectListItem>();
            var allAddons = await _productManager.GetAddOnsOfProductId(productId);
            var sortedAddons = allAddons.OrderBy(x => x.Name);
            foreach (var addon in sortedAddons)
            {
                Items.Add(new SelectListItem()
                {
                    Value = addon.Id,
                    Text = addon.Name,
                    Selected = false
                });
            }
            return Json(Items);
        }

    }
}
